using Domain.Permission.Enums;

namespace Domain.Abstractions;

/// <summary>
/// Service interface for determining location-based access control.
/// Implementations should be provided in the Application or Infrastructure layer.
/// </summary>
public interface ILocationAccessService
{
    /// <summary>
    /// Gets all location IDs that a user can access based on their permission scope.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="scope">The permission scope.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of accessible location IDs.</returns>
    Task<IEnumerable<Guid>> GetAccessibleLocationIdsAsync(
        Guid userId,
        PermissionScope scope,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user can access a specific location based on their permission scope.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="locationId">The location ID to check.</param>
    /// <param name="scope">The permission scope.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the user can access the location, false otherwise.</returns>
    Task<bool> CanAccessLocationAsync(
        Guid userId,
        Guid locationId,
        PermissionScope scope,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the user's current location context ID.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The current location context ID, or null if not set.</returns>
    Task<Guid?> GetCurrentLocationContextAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}

