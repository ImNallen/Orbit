namespace Api.GraphQL.Inputs;

/// <summary>
/// Input type for setting a user's primary location.
/// </summary>
public sealed record SetPrimaryLocationInput(
    Guid UserId,
    Guid LocationId);

