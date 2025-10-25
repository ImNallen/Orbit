using Domain.Abstractions;

namespace Domain.Users.Role;

/// <summary>
/// Event raised when a role is assigned to a user.
/// </summary>
public sealed record RoleAssignedToUserEvent(
    Guid UserId,
    Guid RoleId,
    string RoleName) : DomainEvent;

