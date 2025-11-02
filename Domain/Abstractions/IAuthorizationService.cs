namespace Domain.Abstractions;

/// <summary>
/// Service for loading user authorization data (roles and permissions).
/// </summary>
public interface IAuthorizationService
{
    /// <summary>
    /// Gets the role name for a user.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Role name (e.g., "Admin") or null if user has no role.</returns>
    Task<string?> GetUserRoleAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all permission names for a user (aggregated from all their roles).
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of permission names (e.g., ["users:create", "posts:read"]).</returns>
    Task<List<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);
}

