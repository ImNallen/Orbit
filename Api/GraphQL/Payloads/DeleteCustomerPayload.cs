namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for deleteCustomer mutation.
/// </summary>
public sealed class DeleteCustomerPayload
{
    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<CustomerError> Errors { get; init; } = Array.Empty<CustomerError>();

    public static DeleteCustomerPayload Success(string message) => new()
    {
        IsSuccess = true,
        Message = message
    };

    public static DeleteCustomerPayload Failure(params CustomerError[] errors) => new()
    {
        IsSuccess = false,
        Errors = errors
    };
}

