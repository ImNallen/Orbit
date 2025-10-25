using Domain.Abstractions;
using Domain.Users.Errors;

namespace Domain.Users.Session;

/// <summary>
/// Represents a user session with access and refresh tokens.
/// </summary>
public sealed class Session : Entity
{
    private Session(
        Guid id,
        Guid userId,
        string refreshToken,
        string ipAddress,
        string userAgent,
        DateTime expiresAt)
        : base(id)
    {
        UserId = userId;
        RefreshToken = refreshToken;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
        LastAccessedAt = DateTime.UtcNow;
        IsRevoked = false;
    }

    // EF Core constructor
    private Session() { }

    /// <summary>
    /// The user this session belongs to.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// The refresh token for this session.
    /// </summary>
    public string RefreshToken { get; private set; } = string.Empty;

    /// <summary>
    /// IP address from which the session was created.
    /// </summary>
    public string IpAddress { get; private set; } = string.Empty;

    /// <summary>
    /// User agent string from the client.
    /// </summary>
    public string UserAgent { get; private set; } = string.Empty;

    /// <summary>
    /// When the session was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// When the session expires.
    /// </summary>
    public DateTime ExpiresAt { get; private set; }

    /// <summary>
    /// When the session was last accessed.
    /// </summary>
    public DateTime LastAccessedAt { get; private set; }

    /// <summary>
    /// Whether the session has been revoked.
    /// </summary>
    public bool IsRevoked { get; private set; }

    /// <summary>
    /// When the session was revoked (if applicable).
    /// </summary>
    public DateTime? RevokedAt { get; private set; }

    /// <summary>
    /// Creates a new session.
    /// </summary>
    public static Session Create(
        Guid userId,
        string refreshToken,
        string ipAddress,
        string userAgent,
        TimeSpan expirationDuration)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken);
        ArgumentException.ThrowIfNullOrWhiteSpace(ipAddress);

        return new Session(
            Guid.NewGuid(),
            userId,
            refreshToken,
            ipAddress,
            userAgent ?? "Unknown",
            DateTime.UtcNow.Add(expirationDuration));
    }

    /// <summary>
    /// Checks if the session is valid (not expired and not revoked).
    /// </summary>
    public Result<DomainError> Validate()
    {
        if (IsRevoked)
        {
            return Result<DomainError>.Failure(UserErrors.SessionExpired);
        }

        if (DateTime.UtcNow > ExpiresAt)
        {
            return Result<DomainError>.Failure(UserErrors.SessionExpired);
        }

        return Result<DomainError>.Success();
    }

    /// <summary>
    /// Updates the last accessed timestamp.
    /// </summary>
    public void UpdateLastAccessed()
    {
        LastAccessedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Revokes the session.
    /// </summary>
    public void Revoke()
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if the refresh token matches.
    /// </summary>
    public bool VerifyRefreshToken(string refreshToken)
    {
        return RefreshToken.Equals(refreshToken, StringComparison.Ordinal);
    }
}

