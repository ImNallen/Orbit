namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for suspendCustomer mutation.
/// </summary>
public sealed class SuspendCustomerPayload
{
    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<CustomerError> Errors { get; init; } = Array.Empty<CustomerError>();

    public static SuspendCustomerPayload Success(string message) => new()
    {
        IsSuccess = true,
        Message = message
    };

    public static SuspendCustomerPayload Failure(params CustomerError[] errors) => new()
    {
        IsSuccess = false,
        Errors = errors
    };
}

