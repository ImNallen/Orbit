using Domain.Abstractions;

namespace Domain.Users.Events;

/// <summary>
/// Event raised when a user's password is changed.
/// </summary>
public sealed record PasswordChangedEvent(
    Guid UserId) : DomainEvent;

