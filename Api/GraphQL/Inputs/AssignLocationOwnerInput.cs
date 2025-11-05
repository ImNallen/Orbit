namespace Api.GraphQL.Inputs;

/// <summary>
/// Input type for assigning an owner to a location.
/// </summary>
public sealed record AssignLocationOwnerInput(
    Guid LocationId,
    Guid UserId);

