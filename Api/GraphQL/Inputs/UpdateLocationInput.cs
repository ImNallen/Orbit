namespace Api.GraphQL.Inputs;

/// <summary>
/// Input type for updating a location.
/// </summary>
public sealed record UpdateLocationInput(
    Guid LocationId,
    string Name,
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode);

