namespace Api.GraphQL.Inputs;

/// <summary>
/// Input type for unassigning a user from a location.
/// </summary>
public sealed record UnassignUserFromLocationInput(
    Guid UserId,
    Guid LocationId);

