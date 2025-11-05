namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for adjustStock mutation.
/// </summary>
public sealed class AdjustStockPayload
{
    public string? Message { get; init; }
    public int NewQuantity { get; init; }
    public int AvailableQuantity { get; init; }
    public IReadOnlyList<InventoryError> Errors { get; init; } = Array.Empty<InventoryError>();

    public static AdjustStockPayload Success(string message, int newQuantity, int availableQuantity) =>
        new() { Message = message, NewQuantity = newQuantity, AvailableQuantity = availableQuantity };

    public static AdjustStockPayload Failure(params InventoryError[] errors) =>
        new() { Errors = errors };
}

