using Domain.Abstractions;

namespace Domain.Users.Events;

/// <summary>
/// Event raised when a user is unassigned from a location.
/// </summary>
/// <param name="UserId">The user ID.</param>
/// <param name="LocationId">The location ID.</param>
public sealed record UserUnassignedFromLocationEvent(
    Guid UserId,
    Guid LocationId) : DomainEvent;

