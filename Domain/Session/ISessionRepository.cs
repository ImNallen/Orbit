namespace Domain.Session;

/// <summary>
/// Repository interface for Session aggregate.
/// </summary>
public interface ISessionRepository
{
    /// <summary>
    /// Gets a session by its unique identifier.
    /// </summary>
    /// <param name="id">The session's unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The session if found, otherwise null.</returns>
    Task<Session?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a session by its refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The session if found, otherwise null.</returns>
    Task<Session?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active (non-revoked, non-expired) sessions for a user.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of active sessions.</returns>
    Task<List<Session>> GetActiveSessionsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all sessions (active and inactive) for a user.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of all sessions.</returns>
    Task<List<Session>> GetAllSessionsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets expired sessions that haven't been cleaned up yet.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of expired sessions.</returns>
    Task<List<Session>> GetExpiredSessionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new session to the repository.
    /// </summary>
    /// <param name="session">The session to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(Session session, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing session.
    /// </summary>
    /// <param name="session">The session to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateAsync(Session session, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a session from the repository.
    /// </summary>
    /// <param name="session">The session to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync(Session session, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes all sessions for a user.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes expired sessions (cleanup operation).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of sessions deleted.</returns>
    Task<int> DeleteExpiredSessionsAsync(CancellationToken cancellationToken = default);
}

