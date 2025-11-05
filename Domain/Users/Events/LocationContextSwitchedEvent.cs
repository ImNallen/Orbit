using Domain.Abstractions;

namespace Domain.Users.Events;

/// <summary>
/// Event raised when a user switches their current location context.
/// </summary>
/// <param name="UserId">The user ID.</param>
/// <param name="PreviousLocationId">The previous location ID (null if none).</param>
/// <param name="NewLocationId">The new location ID.</param>
public sealed record LocationContextSwitchedEvent(
    Guid UserId,
    Guid? PreviousLocationId,
    Guid NewLocationId) : DomainEvent;

