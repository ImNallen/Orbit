using Domain.Abstractions;

namespace Domain.Users.User;

/// <summary>
/// Event raised when a user account is locked due to failed login attempts.
/// </summary>
public sealed record AccountLockedEvent(
    Guid UserId,
    DateTime LockedUntil,
    int FailedAttemptCount) : DomainEvent;

