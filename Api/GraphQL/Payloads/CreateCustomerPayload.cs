using Api.GraphQL.Types;

namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for createCustomer mutation.
/// </summary>
public sealed class CreateCustomerPayload
{
    public CustomerType? Customer { get; init; }
    public IReadOnlyList<CustomerError> Errors { get; init; } = Array.Empty<CustomerError>();

    public static CreateCustomerPayload Success(CustomerType customer) =>
        new() { Customer = customer };

    public static CreateCustomerPayload Failure(params CustomerError[] errors) =>
        new() { Errors = errors };
}

