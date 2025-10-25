using Domain.Abstractions;

namespace Domain.Users.User;

/// <summary>
/// Event raised when a user's email is verified.
/// </summary>
public sealed record UserEmailVerifiedEvent(
    Guid UserId,
    string Email) : DomainEvent;

