namespace Api.GraphQL.Inputs;

/// <summary>
/// Input type for assigning a manager to a location.
/// </summary>
public sealed record AssignLocationManagerInput(
    Guid LocationId,
    Guid UserId);

