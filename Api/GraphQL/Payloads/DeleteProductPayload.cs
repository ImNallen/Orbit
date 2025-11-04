namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for deleteProduct mutation.
/// </summary>
public sealed class DeleteProductPayload
{
    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<ProductError> Errors { get; init; } = Array.Empty<ProductError>();

    public static DeleteProductPayload Success(string message) => new()
    {
        IsSuccess = true,
        Message = message
    };

    public static DeleteProductPayload Failure(params ProductError[] errors) => new()
    {
        IsSuccess = false,
        Errors = errors
    };
}

