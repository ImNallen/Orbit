using Domain.Abstractions;
using Domain.Locations;
using Domain.Permission.Enums;
using Domain.Users;
using Domain.Users.Enums;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Service for determining location-based access control.
/// Implements ILocationAccessService from the Domain layer.
/// </summary>
public sealed class LocationAccessService : ILocationAccessService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<LocationAccessService> _logger;

    public LocationAccessService(
        ApplicationDbContext context,
        ILogger<LocationAccessService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Gets all location IDs that a user can access based on their permission scope.
    /// </summary>
    public async Task<IEnumerable<Guid>> GetAccessibleLocationIdsAsync(
        Guid userId,
        PermissionScope scope,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting accessible locations for user {UserId} with scope {Scope}",
            userId, scope);

        return scope switch
        {
            PermissionScope.Global => await GetAllLocationIdsAsync(cancellationToken),
            PermissionScope.Owned => await GetOwnedLocationIdsAsync(userId, cancellationToken),
            PermissionScope.Managed => await GetManagedLocationIdsAsync(userId, cancellationToken),
            PermissionScope.Assigned => await GetAssignedLocationIdsAsync(userId, cancellationToken),
            PermissionScope.Context => await GetContextLocationIdAsync(userId, cancellationToken),
            _ => []
        };
    }

    /// <summary>
    /// Checks if a user can access a specific location based on their permission scope.
    /// </summary>
    public async Task<bool> CanAccessLocationAsync(
        Guid userId,
        Guid locationId,
        PermissionScope scope,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Checking if user {UserId} can access location {LocationId} with scope {Scope}",
            userId, locationId, scope);

        return scope switch
        {
            PermissionScope.Global => true,
            PermissionScope.Owned => await IsLocationOwnedByUserAsync(userId, locationId, cancellationToken),
            PermissionScope.Managed => await IsLocationManagedByUserAsync(userId, locationId, cancellationToken),
            PermissionScope.Assigned => await IsUserAssignedToLocationAsync(userId, locationId, cancellationToken),
            PermissionScope.Context => await IsLocationCurrentContextAsync(userId, locationId, cancellationToken),
            _ => false
        };
    }

    /// <summary>
    /// Gets the user's current location context ID.
    /// </summary>
    public async Task<Guid?> GetCurrentLocationContextAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting current location context for user {UserId}", userId);

        return await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => u.CurrentLocationContextId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    #region Private Helper Methods

    private async Task<IEnumerable<Guid>> GetAllLocationIdsAsync(CancellationToken cancellationToken)
    {
        return await _context.Locations
            .Select(l => l.Id)
            .ToListAsync(cancellationToken);
    }

    private async Task<IEnumerable<Guid>> GetOwnedLocationIdsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await _context.Locations
            .Where(l => l.OwnerId == userId)
            .Select(l => l.Id)
            .ToListAsync(cancellationToken);
    }

    private async Task<IEnumerable<Guid>> GetManagedLocationIdsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await _context.Locations
            .Where(l => l.ManagerId == userId)
            .Select(l => l.Id)
            .ToListAsync(cancellationToken);
    }

    private async Task<IEnumerable<Guid>> GetAssignedLocationIdsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await _context.UserLocationAssignments
            .Where(ula => ula.UserId == userId && ula.Status == AssignmentStatus.Active)
            .Select(ula => ula.LocationId)
            .ToListAsync(cancellationToken);
    }

    private async Task<IEnumerable<Guid>> GetContextLocationIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        Guid? contextId = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => u.CurrentLocationContextId)
            .FirstOrDefaultAsync(cancellationToken);

        return contextId.HasValue ? [contextId.Value] : [];
    }

    private async Task<bool> IsLocationOwnedByUserAsync(
        Guid userId,
        Guid locationId,
        CancellationToken cancellationToken)
    {
        return await _context.Locations
            .AnyAsync(l => l.Id == locationId && l.OwnerId == userId, cancellationToken);
    }

    private async Task<bool> IsLocationManagedByUserAsync(
        Guid userId,
        Guid locationId,
        CancellationToken cancellationToken)
    {
        return await _context.Locations
            .AnyAsync(l => l.Id == locationId && l.ManagerId == userId, cancellationToken);
    }

    private async Task<bool> IsUserAssignedToLocationAsync(
        Guid userId,
        Guid locationId,
        CancellationToken cancellationToken)
    {
        return await _context.UserLocationAssignments
            .AnyAsync(ula =>
                ula.UserId == userId &&
                ula.LocationId == locationId &&
                ula.Status == AssignmentStatus.Active,
                cancellationToken);
    }

    private async Task<bool> IsLocationCurrentContextAsync(
        Guid userId,
        Guid locationId,
        CancellationToken cancellationToken)
    {
        Guid? contextId = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => u.CurrentLocationContextId)
            .FirstOrDefaultAsync(cancellationToken);

        return contextId == locationId;
    }

    #endregion
}

