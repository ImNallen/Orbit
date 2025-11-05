using Domain.Abstractions;
using Domain.UserLocations.Enums;
using Domain.Users.Enums;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Service for determining location-based access control.
/// Simplified to only check UserLocationAssignment.
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
    /// Gets all location IDs that a user can access based on their assignments.
    /// </summary>
    public async Task<IEnumerable<Guid>> GetAccessibleLocationIdsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting accessible locations for user {UserId}", userId);

        return await _context.UserLocationAssignments
            .Where(ula => ula.UserId == userId && ula.Status == AssignmentStatus.Active)
            .Select(ula => ula.LocationId)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Checks if a user can access a specific location based on their assignments.
    /// </summary>
    public async Task<bool> CanAccessLocationAsync(
        Guid userId,
        Guid locationId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Checking if user {UserId} can access location {LocationId}",
            userId, locationId);

        return await _context.UserLocationAssignments
            .AnyAsync(ula =>
                ula.UserId == userId &&
                ula.LocationId == locationId &&
                ula.Status == AssignmentStatus.Active,
                cancellationToken);
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
}

