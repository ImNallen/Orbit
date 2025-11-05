namespace Api.GraphQL.Inputs;

/// <summary>
/// Input type for removing the owner from a location.
/// </summary>
public sealed record RemoveLocationOwnerInput(
    Guid LocationId);

