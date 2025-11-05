using Domain.Abstractions;

namespace Domain.Locations.Events;

/// <summary>
/// Event raised when a new location is created.
/// </summary>
public sealed record LocationCreatedEvent(
    Guid LocationId,
    string Name,
    string Type) : DomainEvent;

