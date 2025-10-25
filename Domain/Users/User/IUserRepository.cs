namespace Domain.Users.User;

/// <summary>
/// Repository interface for User aggregate.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Gets a user by their unique identifier.
    /// </summary>
    /// <param name="id">The user's unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user if found, otherwise null.</returns>
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by their email address.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user if found, otherwise null.</returns>
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by their email verification token.
    /// </summary>
    /// <param name="token">The email verification token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user if found, otherwise null.</returns>
    Task<User?> GetByEmailVerificationTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by their password reset token.
    /// </summary>
    /// <param name="token">The password reset token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user if found, otherwise null.</returns>
    Task<User?> GetByPasswordResetTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all users with pagination support.
    /// </summary>
    /// <param name="skip">Number of users to skip.</param>
    /// <param name="take">Number of users to take.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of users.</returns>
    Task<List<User>> GetAllAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of users.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Total number of users.</returns>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users by their status.
    /// </summary>
    /// <param name="status">The user status to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of users with the specified status.</returns>
    Task<List<User>> GetByStatusAsync(UserStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users by role ID.
    /// </summary>
    /// <param name="roleId">The role identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of users with the specified role.</returns>
    Task<List<User>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user with the specified email exists.
    /// </summary>
    /// <param name="email">The email to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if a user with the email exists, otherwise false.</returns>
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new user to the repository.
    /// </summary>
    /// <param name="user">The user to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="user">The user to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a user from the repository.
    /// </summary>
    /// <param name="user">The user to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the password history for a user.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="limit">Maximum number of password history entries to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of password history entries.</returns>
    Task<List<PasswordHistory>> GetPasswordHistoryAsync(Guid userId, int limit = 5, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a password history entry for a user.
    /// </summary>
    /// <param name="passwordHistory">The password history entry to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddPasswordHistoryAsync(PasswordHistory passwordHistory, CancellationToken cancellationToken = default);
}

