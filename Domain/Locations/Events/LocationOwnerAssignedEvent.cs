using Domain.Abstractions;

namespace Domain.Locations.Events;

/// <summary>
/// Event raised when an owner is assigned to a location.
/// </summary>
/// <param name="LocationId">The location ID.</param>
/// <param name="OwnerId">The owner user ID.</param>
public sealed record LocationOwnerAssignedEvent(
    Guid LocationId,
    Guid OwnerId) : DomainEvent;

