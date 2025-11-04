namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for deactivateProduct mutation.
/// </summary>
public sealed class DeactivateProductPayload
{
    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<ProductError> Errors { get; init; } = Array.Empty<ProductError>();

    public static DeactivateProductPayload Success(string message) => new()
    {
        IsSuccess = true,
        Message = message
    };

    public static DeactivateProductPayload Failure(params ProductError[] errors) => new()
    {
        IsSuccess = false,
        Errors = errors
    };
}

