using Api.GraphQL.Types;

namespace Api.GraphQL.Inputs;

/// <summary>
/// Input type for changing location status.
/// </summary>
public sealed record ChangeLocationStatusInput(
    Guid LocationId,
    LocationStatusType Status);

