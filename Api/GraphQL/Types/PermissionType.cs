namespace Api.GraphQL.Types;

/// <summary>
/// GraphQL type for permission information.
/// </summary>
public sealed class PermissionType
{
    public Guid PermissionId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Resource { get; init; } = string.Empty;
    public string Action { get; init; } = string.Empty;
}

