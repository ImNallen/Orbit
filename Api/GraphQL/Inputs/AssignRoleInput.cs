namespace Api.GraphQL.Inputs;

/// <summary>
/// Input for assignRole mutation.
/// </summary>
public sealed record AssignRoleInput(Guid UserId, Guid RoleId);

