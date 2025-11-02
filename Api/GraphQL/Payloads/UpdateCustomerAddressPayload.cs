namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for updateCustomerAddress mutation.
/// </summary>
public sealed class UpdateCustomerAddressPayload
{
    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<CustomerError> Errors { get; init; } = Array.Empty<CustomerError>();

    public static UpdateCustomerAddressPayload Success(string message) => new()
    {
        IsSuccess = true,
        Message = message
    };

    public static UpdateCustomerAddressPayload Failure(params CustomerError[] errors) => new()
    {
        IsSuccess = false,
        Errors = errors
    };
}

