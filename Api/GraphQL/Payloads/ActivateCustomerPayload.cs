namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for activateCustomer mutation.
/// </summary>
public sealed class ActivateCustomerPayload
{
    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<CustomerError> Errors { get; init; } = Array.Empty<CustomerError>();

    public static ActivateCustomerPayload Success(string message) => new()
    {
        IsSuccess = true,
        Message = message
    };

    public static ActivateCustomerPayload Failure(params CustomerError[] errors) => new()
    {
        IsSuccess = false,
        Errors = errors
    };
}

