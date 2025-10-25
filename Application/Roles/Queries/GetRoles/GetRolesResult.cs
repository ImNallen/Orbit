namespace Application.Roles.Queries.GetRoles;

/// <summary>
/// Result for GetRolesQuery.
/// </summary>
public sealed record GetRolesResult(IReadOnlyList<RoleDto> Roles);

/// <summary>
/// DTO for role information.
/// </summary>
public sealed record RoleDto(
    Guid RoleId,
    string Name,
    string Description,
    IReadOnlyList<Guid> PermissionIds);

