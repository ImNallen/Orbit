using Api.GraphQL.Types;

namespace Api.GraphQL.Inputs;

/// <summary>
/// Input type for creating a location.
/// </summary>
public sealed record CreateLocationInput(
    string Name,
    LocationTypeType Type,
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode);

