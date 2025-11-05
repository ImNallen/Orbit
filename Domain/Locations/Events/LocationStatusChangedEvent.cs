using Domain.Abstractions;

namespace Domain.Locations.Events;

/// <summary>
/// Event raised when a location's status changes.
/// </summary>
public sealed record LocationStatusChangedEvent(
    Guid LocationId,
    string OldStatus,
    string NewStatus) : DomainEvent;

