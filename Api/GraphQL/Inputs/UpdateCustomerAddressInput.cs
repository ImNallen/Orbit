namespace Api.GraphQL.Inputs;

/// <summary>
/// Input type for updating customer address.
/// </summary>
public sealed record UpdateCustomerAddressInput(
    Guid CustomerId,
    string Street,
    string City,
    string? State,
    string Country,
    string ZipCode);

