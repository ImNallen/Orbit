namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for transferStock mutation.
/// </summary>
public sealed class TransferStockPayload
{
    public string? Message { get; init; }
    public Guid? FromInventoryId { get; init; }
    public Guid? ToInventoryId { get; init; }
    public int Quantity { get; init; }
    public int FromNewQuantity { get; init; }
    public int ToNewQuantity { get; init; }
    public IReadOnlyList<InventoryError> Errors { get; init; } = Array.Empty<InventoryError>();

    public static TransferStockPayload Success(
        Guid fromInventoryId,
        Guid toInventoryId,
        int quantity,
        int fromNewQuantity,
        int toNewQuantity) => new()
    {
        Message = $"Successfully transferred {quantity} units",
        FromInventoryId = fromInventoryId,
        ToInventoryId = toInventoryId,
        Quantity = quantity,
        FromNewQuantity = fromNewQuantity,
        ToNewQuantity = toNewQuantity
    };

    public static TransferStockPayload Failure(params InventoryError[] errors) => new()
    {
        Errors = errors
    };
}

