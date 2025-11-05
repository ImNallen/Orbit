using Domain.Abstractions;

namespace Domain.Inventory.Events;

/// <summary>
/// Event raised when inventory stock is adjusted.
/// </summary>
public sealed record StockAdjustedEvent(
    Guid InventoryId,
    Guid ProductId,
    Guid LocationId,
    int OldQuantity,
    int NewQuantity,
    int Adjustment,
    string Reason) : DomainEvent;

