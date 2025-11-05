using System.Security.Claims;
using Application.Services;
using Domain.Abstractions;
using Domain.Permission.Enums;
using Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Service for accessing information about the currently authenticated user.
/// </summary>
public sealed class CurrentUserService : ICurrentUserService
{
    private readonly Microsoft.AspNetCore.Http.IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _context;
    private readonly ILocationAccessService _locationAccessService;
    private readonly ILogger<CurrentUserService> _logger;

    public CurrentUserService(
        Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor,
        ApplicationDbContext context,
        ILocationAccessService locationAccessService,
        ILogger<CurrentUserService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
        _locationAccessService = locationAccessService;
        _logger = logger;
    }

    /// <summary>
    /// Gets the current user's ID from the HTTP context.
    /// </summary>
    public Guid GetUserId()
    {
        ClaimsPrincipal? user = _httpContextAccessor.HttpContext?.User;

        if (user is null || !user.Identity?.IsAuthenticated == true)
        {
            throw new InvalidOperationException("User is not authenticated.");
        }

        string? userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
        {
            throw new InvalidOperationException("User ID claim is missing or invalid.");
        }

        return userId;
    }

    /// <summary>
    /// Gets the current user's email from the HTTP context.
    /// </summary>
    public string? GetUserEmail()
    {
        ClaimsPrincipal? user = _httpContextAccessor.HttpContext?.User;
        return user?.FindFirst(ClaimTypes.Email)?.Value;
    }

    /// <summary>
    /// Gets the permission scope for a specific permission for the current user.
    /// </summary>
    public async Task<PermissionScope?> GetPermissionScopeAsync(
        string permissionName,
        CancellationToken cancellationToken = default)
    {
        if (!IsAuthenticated())
        {
            _logger.LogWarning("Attempted to get permission scope for unauthenticated user");
            return null;
        }

        // Get the permission from the database
        Domain.Permission.Permission? permission = await _context.Permissions
            .Where(p => p.Name == permissionName)
            .FirstOrDefaultAsync(cancellationToken);

        if (permission is null)
        {
            _logger.LogWarning("Permission {PermissionName} not found", permissionName);
            return null;
        }

        // Check if the user has this permission
        ClaimsPrincipal? user = _httpContextAccessor.HttpContext?.User;
        IEnumerable<string> userPermissions = user?
            .FindAll(claim => claim.Type == "permission")
            .Select(claim => claim.Value) ?? [];

        if (!userPermissions.Contains(permissionName))
        {
            _logger.LogDebug("User does not have permission {PermissionName}", permissionName);
            return null;
        }

        return permission.Scope;
    }

    /// <summary>
    /// Gets all location IDs that the current user can access for a specific permission.
    /// Uses role-based scope resolution to determine the effective scope.
    /// </summary>
    public async Task<IEnumerable<Guid>> GetAccessibleLocationIdsAsync(
        string permissionName,
        CancellationToken cancellationToken = default)
    {
        if (!IsAuthenticated())
        {
            _logger.LogWarning("Attempted to get accessible locations for unauthenticated user");
            return [];
        }

        // Get the permission scope (base scope from permission definition)
        PermissionScope? baseScope = await GetPermissionScopeAsync(permissionName, cancellationToken);

        if (baseScope is null)
        {
            _logger.LogDebug("User does not have permission {PermissionName}, returning empty location list", permissionName);
            return [];
        }

        // Get the user ID
        Guid userId = GetUserId();

        // Determine the effective scope based on user's role
        PermissionScope effectiveScope = await GetEffectiveScopeAsync(userId, baseScope.Value, cancellationToken);

        // Get accessible locations based on effective scope
        IEnumerable<Guid> locationIds = await _locationAccessService.GetAccessibleLocationIdsAsync(
            userId,
            effectiveScope,
            cancellationToken);

        _logger.LogInformation(
            "User {UserId} has access to {Count} locations for permission {PermissionName} (base scope: {BaseScope}, effective scope: {EffectiveScope})",
            userId,
            locationIds.Count(),
            permissionName,
            baseScope.Value,
            effectiveScope);

        return locationIds;
    }

    /// <summary>
    /// Determines the effective permission scope based on the user's role.
    /// Uses role hierarchy: HQ Admin (Global) > Store Owner (Owned) > Store Manager (Managed) > Employee (Assigned).
    /// </summary>
    private async Task<PermissionScope> GetEffectiveScopeAsync(
        Guid userId,
        PermissionScope baseScope,
        CancellationToken cancellationToken)
    {
        // If base scope is already Global or Context, use it as-is
        if (baseScope == PermissionScope.Global || baseScope == PermissionScope.Context)
        {
            return baseScope;
        }

        // Get the user's role name
        string? roleName = await _context.Users
            .Where(u => u.Id == userId)
            .Join(_context.Roles, u => u.RoleId, r => r.Id, (u, r) => r.Name)
            .FirstOrDefaultAsync(cancellationToken);

        if (string.IsNullOrEmpty(roleName))
        {
            _logger.LogWarning("User {UserId} has no role, using base scope {BaseScope}", userId, baseScope);
            return baseScope;
        }

        // Apply role-based scope resolution
        // Role hierarchy determines the effective scope
        PermissionScope effectiveScope = roleName switch
        {
            "HQ Admin" => PermissionScope.Global,
            "Store Owner" => PermissionScope.Owned,
            "Store Manager" => PermissionScope.Managed,
            "Employee" => PermissionScope.Assigned,
            "Read-Only User" => PermissionScope.Assigned,
            _ => baseScope // Fallback to base scope for unknown roles
        };

        _logger.LogInformation(
            "User {UserId} with role {RoleName}: base scope {BaseScope} → effective scope {EffectiveScope}",
            userId, roleName, baseScope, effectiveScope);

        return effectiveScope;
    }

    /// <summary>
    /// Checks if the current user is authenticated.
    /// </summary>
    public bool IsAuthenticated()
    {
        ClaimsPrincipal? user = _httpContextAccessor.HttpContext?.User;
        return user?.Identity?.IsAuthenticated == true;
    }
}

