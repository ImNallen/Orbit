namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for activateProduct mutation.
/// </summary>
public sealed class ActivateProductPayload
{
    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<ProductError> Errors { get; init; } = Array.Empty<ProductError>();

    public static ActivateProductPayload Success(string message) => new()
    {
        IsSuccess = true,
        Message = message
    };

    public static ActivateProductPayload Failure(params ProductError[] errors) => new()
    {
        IsSuccess = false,
        Errors = errors
    };
}

