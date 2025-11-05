using Domain.Abstractions;

namespace Domain.Inventory.Events;

/// <summary>
/// Event raised when stock is reserved.
/// </summary>
public sealed record StockReservedEvent(
    Guid InventoryId,
    Guid ProductId,
    Guid LocationId,
    int Quantity,
    int TotalReserved) : DomainEvent;

