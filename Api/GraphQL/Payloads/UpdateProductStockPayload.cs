namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for updateProductStock mutation.
/// </summary>
public sealed class UpdateProductStockPayload
{
    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    public int NewStockQuantity { get; init; }
    public IReadOnlyList<ProductError> Errors { get; init; } = Array.Empty<ProductError>();

    public static UpdateProductStockPayload Success(string message, int newStockQuantity) => new()
    {
        IsSuccess = true,
        Message = message,
        NewStockQuantity = newStockQuantity
    };

    public static UpdateProductStockPayload Failure(params ProductError[] errors) => new()
    {
        IsSuccess = false,
        Errors = errors
    };
}

