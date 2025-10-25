using Domain.Abstractions;

namespace Domain.Users.User;

/// <summary>
/// Event raised when a user logs out.
/// </summary>
public sealed record UserLoggedOutEvent(
    Guid UserId,
    Guid SessionId) : DomainEvent;

