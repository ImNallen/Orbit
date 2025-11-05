namespace Api.GraphQL.Inputs;

/// <summary>
/// Input type for removing the manager from a location.
/// </summary>
public sealed record RemoveLocationManagerInput(
    Guid LocationId);

