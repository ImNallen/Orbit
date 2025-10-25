using Domain.Abstractions;

namespace Domain.Users.User;

/// <summary>
/// Event raised when a new user is registered.
/// </summary>
public sealed record UserRegisteredEvent(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName) : DomainEvent;

