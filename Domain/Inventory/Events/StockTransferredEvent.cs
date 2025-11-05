using Domain.Abstractions;

namespace Domain.Inventory.Events;

/// <summary>
/// Event raised when stock is transferred between locations.
/// </summary>
public sealed record StockTransferredEvent(
    Guid FromInventoryId,
    Guid ToInventoryId,
    Guid ProductId,
    Guid FromLocationId,
    Guid ToLocationId,
    int Quantity) : DomainEvent;

