namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for updateProduct mutation.
/// </summary>
public sealed class UpdateProductPayload
{
    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<ProductError> Errors { get; init; } = Array.Empty<ProductError>();

    public static UpdateProductPayload Success(string message) => new()
    {
        IsSuccess = true,
        Message = message
    };

    public static UpdateProductPayload Failure(params ProductError[] errors) => new()
    {
        IsSuccess = false,
        Errors = errors
    };
}

