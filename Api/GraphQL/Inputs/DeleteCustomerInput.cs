namespace Api.GraphQL.Inputs;

/// <summary>
/// Input for deleteCustomer mutation.
/// </summary>
public sealed record DeleteCustomerInput(Guid CustomerId);

