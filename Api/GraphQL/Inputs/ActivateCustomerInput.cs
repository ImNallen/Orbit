namespace Api.GraphQL.Inputs;

/// <summary>
/// Input for activateCustomer mutation.
/// </summary>
public sealed record ActivateCustomerInput(Guid CustomerId);

