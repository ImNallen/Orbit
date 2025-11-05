using Domain.Abstractions;

namespace Domain.Locations.Events;

/// <summary>
/// Event raised when an owner is removed from a location.
/// </summary>
/// <param name="LocationId">The location ID.</param>
/// <param name="PreviousOwnerId">The previous owner user ID.</param>
public sealed record LocationOwnerRemovedEvent(
    Guid LocationId,
    Guid PreviousOwnerId) : DomainEvent;

