namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for updateCustomerContactInfo mutation.
/// </summary>
public sealed class UpdateCustomerContactInfoPayload
{
    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<CustomerError> Errors { get; init; } = Array.Empty<CustomerError>();

    public static UpdateCustomerContactInfoPayload Success(string message) => new()
    {
        IsSuccess = true,
        Message = message
    };

    public static UpdateCustomerContactInfoPayload Failure(params CustomerError[] errors) => new()
    {
        IsSuccess = false,
        Errors = errors
    };
}

