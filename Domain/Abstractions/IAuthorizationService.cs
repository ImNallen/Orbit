namespace Domain.Abstractions;

/// <summary>
/// Service for loading user authorization data (roles and permissions).
/// </summary>
public interface IAuthorizationService
{
    /// <summary>
    /// Gets all role names for a user.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of role names (e.g., ["Admin", "User"]).</returns>
    Task<List<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all permission names for a user (aggregated from all their roles).
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of permission names (e.g., ["users:create", "posts:read"]).</returns>
    Task<List<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);
}

