namespace Api.GraphQL.Inputs;

/// <summary>
/// Input type for updating customer contact information.
/// </summary>
public sealed record UpdateCustomerContactInfoInput(
    Guid CustomerId,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber);

