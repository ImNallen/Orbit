using Domain.Permission.Enums;

namespace Application.Services;

/// <summary>
/// Service for accessing information about the currently authenticated user.
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
    /// Gets the permission scope for a specific permission for the current user.
    /// </summary>
    /// <param name="permissionName">The permission name (e.g., "customers:read").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The permission scope, or null if the user doesn't have the permission.</returns>
    Task<PermissionScope?> GetPermissionScopeAsync(
        string permissionName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all location IDs that the current user can access for a specific permission.
    /// </summary>
    /// <param name="permissionName">The permission name (e.g., "customers:read").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of accessible location IDs. Returns empty collection if user doesn't have the permission.</returns>
    Task<IEnumerable<Guid>> GetAccessibleLocationIdsAsync(
        string permissionName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the current user is authenticated.
    /// </summary>
    /// <returns>True if the user is authenticated, false otherwise.</returns>
    bool IsAuthenticated();
}

