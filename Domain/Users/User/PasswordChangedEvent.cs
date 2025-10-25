using Domain.Abstractions;

namespace Domain.Users.User;

/// <summary>
/// Event raised when a user's password is changed.
/// </summary>
public sealed record PasswordChangedEvent(
    Guid UserId) : DomainEvent;

