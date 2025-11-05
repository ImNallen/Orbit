using Domain.Abstractions;

namespace Domain.Locations.Events;

/// <summary>
/// Event raised when a manager is removed from a location.
/// </summary>
/// <param name="LocationId">The location ID.</param>
/// <param name="PreviousManagerId">The previous manager user ID.</param>
public sealed record LocationManagerRemovedEvent(
    Guid LocationId,
    Guid PreviousManagerId) : DomainEvent;

