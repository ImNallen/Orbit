using Api.GraphQL.Types;

namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for inventories by product query.
/// </summary>
public sealed class InventoriesPayload
{
    public IReadOnlyList<InventoryType> Inventories { get; init; } = Array.Empty<InventoryType>();
    public int TotalQuantity { get; init; }
    public int TotalReservedQuantity { get; init; }
    public int TotalAvailableQuantity { get; init; }
}

