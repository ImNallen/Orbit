namespace Api.GraphQL.Inputs;

/// <summary>
/// Input type for assigning a user to a location.
/// </summary>
public sealed record AssignUserToLocationInput(
    Guid UserId,
    Guid LocationId,
    Guid? LocationRoleId = null,
    bool IsPrimaryLocation = false);

