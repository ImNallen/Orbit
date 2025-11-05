namespace Api.GraphQL.Inputs;

/// <summary>
/// Input type for reserving stock.
/// </summary>
public sealed record ReserveStockInput(
    Guid InventoryId,
    int Quantity);

