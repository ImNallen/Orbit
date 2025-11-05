using System.Security.Claims;
using Application.Services;
using Domain.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Service for accessing information about the currently authenticated user.
/// Simplified to remove scope logic.
/// </summary>
public sealed class CurrentUserService : ICurrentUserService
{
    private readonly Microsoft.AspNetCore.Http.IHttpContextAccessor _httpContextAccessor;
    private readonly ILocationAccessService _locationAccessService;
    private readonly ILogger<CurrentUserService> _logger;

    public CurrentUserService(
        Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor,
        ILocationAccessService locationAccessService,
        ILogger<CurrentUserService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
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
    /// Gets all location IDs that the current user can access.
    /// Simplified to only check UserLocationAssignment.
    /// </summary>
    public async Task<IEnumerable<Guid>> GetAccessibleLocationIdsAsync(
        CancellationToken cancellationToken = default)
    {
        if (!IsAuthenticated())
        {
            _logger.LogWarning("Attempted to get accessible locations for unauthenticated user");
            return [];
        }

        Guid userId = GetUserId();

        IEnumerable<Guid> locationIds = await _locationAccessService.GetAccessibleLocationIdsAsync(
            userId,
            cancellationToken);

        _logger.LogInformation(
            "User {UserId} has access to {Count} locations",
            userId,
            locationIds.Count());

        return locationIds;
    }

    /// <summary>
    /// Checks if the current user is authenticated.
    /// </summary>
    public bool IsAuthenticated()
    {
        ClaimsPrincipal? user = _httpContextAccessor.HttpContext?.User;
        return user?.Identity?.IsAuthenticated == true;
    }

    /// <summary>
    /// Gets the current user's current location context ID.
    /// This is the location the user is currently working at.
    /// </summary>
    public async Task<Guid?> GetCurrentLocationContextIdAsync(
        CancellationToken cancellationToken = default)
    {
        if (!IsAuthenticated())
        {
            _logger.LogWarning("Attempted to get current location context for unauthenticated user");
            return null;
        }

        Guid userId = GetUserId();

        Guid? currentLocationContextId = await _locationAccessService.GetCurrentLocationContextAsync(
            userId,
            cancellationToken);

        _logger.LogDebug(
            "User {UserId} current location context: {LocationId}",
            userId,
            currentLocationContextId?.ToString() ?? "None");

        return currentLocationContextId;
    }
}

