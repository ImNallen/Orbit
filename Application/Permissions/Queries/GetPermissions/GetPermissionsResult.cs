namespace Application.Permissions.Queries.GetPermissions;

/// <summary>
/// Result for GetPermissionsQuery.
/// </summary>
public sealed record GetPermissionsResult(IReadOnlyList<PermissionDto> Permissions);

/// <summary>
/// DTO for permission information.
/// </summary>
public sealed record PermissionDto(
    Guid PermissionId,
    string Name,
    string Description,
    string Resource,
    string Action);

