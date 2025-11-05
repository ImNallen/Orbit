using Domain.Abstractions;

namespace Domain.Locations.Events;

/// <summary>
/// Event raised when a location's information is updated.
/// </summary>
public sealed record LocationUpdatedEvent(
    Guid LocationId,
    string Name) : DomainEvent;

