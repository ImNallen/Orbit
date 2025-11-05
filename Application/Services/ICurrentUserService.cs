namespace Application.Services;

/// <summary>
/// Service for accessing information about the currently authenticated user.
/// Simplified to remove scope logic.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current user's ID from the HTTP context.
    /// </summary>
    /// <returns>The current user's ID.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the user is not authenticated.</exception>
    Guid GetUserId();

    /// <summary>
    /// Gets the current user's email from the HTTP context.
    /// </summary>
    /// <returns>The current user's email, or null if not available.</returns>
    string? GetUserEmail();

    /// <summary>
    /// Gets all location IDs that the current user can access.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of accessible location IDs.</returns>
    Task<IEnumerable<Guid>> GetAccessibleLocationIdsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the current user is authenticated.
    /// </summary>
    /// <returns>True if the user is authenticated, false otherwise.</returns>
    bool IsAuthenticated();

    /// <summary>
    /// Gets the current user's current location context ID.
    /// This is the location the user is currently working at.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The current location context ID, or null if not set.</returns>
    Task<Guid?> GetCurrentLocationContextIdAsync(
        CancellationToken cancellationToken = default);
}

