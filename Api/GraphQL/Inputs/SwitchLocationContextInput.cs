namespace Api.GraphQL.Inputs;

/// <summary>
/// Input type for switching a user's location context.
/// </summary>
public sealed record SwitchLocationContextInput(
    Guid UserId,
    Guid LocationId);

