using Domain.Abstractions;

namespace Domain.Locations.Events;

/// <summary>
/// Event raised when a manager is assigned to a location.
/// </summary>
/// <param name="LocationId">The location ID.</param>
/// <param name="ManagerId">The manager user ID.</param>
public sealed record LocationManagerAssignedEvent(
    Guid LocationId,
    Guid ManagerId) : DomainEvent;

