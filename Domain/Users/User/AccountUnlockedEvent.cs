using Domain.Abstractions;

namespace Domain.Users.User;

/// <summary>
/// Event raised when a user account is unlocked.
/// </summary>
public sealed record AccountUnlockedEvent(
    Guid UserId) : DomainEvent;

