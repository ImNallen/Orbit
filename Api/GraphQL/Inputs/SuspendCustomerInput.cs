namespace Api.GraphQL.Inputs;

/// <summary>
/// Input for suspendCustomer mutation.
/// </summary>
public sealed record SuspendCustomerInput(Guid CustomerId);

