namespace Api.GraphQL.Inputs;

/// <summary>
/// Input type for assigning a user to a location.
/// </summary>
public sealed record AssignUserToLocationInput(
    Guid UserId,
    Guid LocationId,
    bool IsPrimaryLocation = false);

