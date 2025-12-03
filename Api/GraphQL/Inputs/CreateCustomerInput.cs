namespace Api.GraphQL.Inputs;

/// <summary>
/// Input type for creating a customer.
/// </summary>
public sealed record CreateCustomerInput(
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string Street,
    string City,
    string? State,
    string Country,
    string ZipCode);

