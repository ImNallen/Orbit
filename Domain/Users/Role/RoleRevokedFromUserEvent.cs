using Domain.Abstractions;

namespace Domain.Users.Role;

/// <summary>
/// Event raised when a role is revoked from a user.
/// </summary>
public sealed record RoleRevokedFromUserEvent(
    Guid UserId,
    Guid RoleId,
    string RoleName) : DomainEvent;

