using Domain.Abstractions;

namespace Domain.Users.Role;

/// <summary>
/// Event raised when a user's role is changed.
/// </summary>
public sealed record RoleChangedEvent(
    Guid UserId,
    Guid OldRoleId,
    Guid NewRoleId,
    string NewRoleName) : DomainEvent;

