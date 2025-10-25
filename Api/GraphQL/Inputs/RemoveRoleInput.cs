namespace Api.GraphQL.Inputs;

/// <summary>
/// Input for removeRole mutation.
/// </summary>
public sealed record RemoveRoleInput(Guid UserId, Guid RoleId);

