using Domain.Abstractions;

namespace Domain.Users.User;

/// <summary>
/// Event raised when a user successfully logs in.
/// </summary>
public sealed record UserLoggedInEvent(
    Guid UserId,
    Guid SessionId,
    string IpAddress) : DomainEvent;

