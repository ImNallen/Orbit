namespace Api.GraphQL.Types;

/// <summary>
/// GraphQL type for role information.
/// </summary>
public sealed class RoleType
{
    public Guid RoleId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public IReadOnlyList<Guid> PermissionIds { get; init; } = Array.Empty<Guid>();
}

