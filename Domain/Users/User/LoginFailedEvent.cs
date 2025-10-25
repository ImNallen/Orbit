using Domain.Abstractions;

namespace Domain.Users.User;

/// <summary>
/// Event raised when a login attempt fails.
/// </summary>
public sealed record LoginFailedEvent(
    Guid UserId,
    string Email,
    string IpAddress,
    int FailedAttemptCount) : DomainEvent;

