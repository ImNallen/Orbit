using Domain.Abstractions;

namespace Domain.Role.Events;

/// <summary>
/// Event raised when a user's role is changed.
/// </summary>
public sealed record RoleChangedEvent(
    Guid UserId,
    Guid OldRoleId,
    Guid NewRoleId,
    string NewRoleName) : DomainEvent;

