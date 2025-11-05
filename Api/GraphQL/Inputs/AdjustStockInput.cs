namespace Api.GraphQL.Inputs;

/// <summary>
/// Input type for adjusting stock quantity.
/// </summary>
public sealed record AdjustStockInput(
    Guid InventoryId,
    int Adjustment,
    string Reason);

