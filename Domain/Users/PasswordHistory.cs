using Domain.Abstractions;
using Domain.Users.ValueObjects;

namespace Domain.Users;

/// <summary>
/// Represents a historical password entry for a user.
/// Used to prevent password reuse.
/// </summary>
public sealed class PasswordHistory : Entity
{
    private PasswordHistory(Guid id, Guid userId, PasswordHash passwordHash, DateTime createdAt)
        : base(id)
    {
        UserId = userId;
        PasswordHash = passwordHash;
        CreatedAt = createdAt;
    }

    // EF Core constructor
    private PasswordHistory() { }

    /// <summary>
    /// The user this password history entry belongs to.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// The hashed password.
    /// </summary>
    public PasswordHash PasswordHash { get; private set; } = null!;

    /// <summary>
    /// When this password was set.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Creates a new password history entry.
    /// </summary>
    public static PasswordHistory Create(Guid userId, PasswordHash passwordHash)
    {
        return new PasswordHistory(
            Guid.CreateVersion7(),
            userId,
            passwordHash,
            DateTime.UtcNow);
    }
}

