using Domain.Abstractions;

namespace Domain.Inventory.Events;

/// <summary>
/// Event raised when a stock reservation is released.
/// </summary>
public sealed record StockReservationReleasedEvent(
    Guid InventoryId,
    Guid ProductId,
    Guid LocationId,
    int Quantity,
    int TotalReserved) : DomainEvent;

