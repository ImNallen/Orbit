using Domain.Abstractions;

namespace Domain.Users.Events;

/// <summary>
/// Event raised when a user is assigned to a location.
/// </summary>
/// <param name="UserId">The user ID.</param>
/// <param name="LocationId">The location ID.</param>
/// <param name="IsPrimaryLocation">Whether this is the user's primary location.</param>
public sealed record UserAssignedToLocationEvent(
    Guid UserId,
    Guid LocationId,
    bool IsPrimaryLocation) : DomainEvent;

