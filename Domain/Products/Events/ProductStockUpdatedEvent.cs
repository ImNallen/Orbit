using Domain.Abstractions;

namespace Domain.Products.Events;

/// <summary>
/// Event raised when a product's stock quantity is updated.
/// </summary>
public sealed record ProductStockUpdatedEvent(
    Guid ProductId,
    int NewStockQuantity) : DomainEvent;

