namespace Api.GraphQL.Types;

/// <summary>
/// GraphQL type for inventory with full details.
/// </summary>
public sealed class InventoryType
{
    public Guid InventoryId { get; init; }
    public Guid ProductId { get; init; }
    public Guid LocationId { get; init; }
    public int Quantity { get; init; }
    public int ReservedQuantity { get; init; }
    public int AvailableQuantity { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

