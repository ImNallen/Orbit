using Domain.Abstractions;

namespace Domain.Products.Events;

/// <summary>
/// Event raised when a new product is created.
/// </summary>
public sealed record ProductCreatedEvent(
    Guid ProductId,
    string Name,
    string Sku,
    decimal Price) : DomainEvent;

