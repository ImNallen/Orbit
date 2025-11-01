using Domain.Abstractions;

namespace Domain.Users.Events;

/// <summary>
/// Domain event raised when a user's profile is updated.
/// </summary>
public sealed record UserProfileUpdatedEvent(
    Guid UserId,
    string FirstName,
    string LastName) : DomainEvent;

