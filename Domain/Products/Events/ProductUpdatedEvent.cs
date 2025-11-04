using Domain.Abstractions;

namespace Domain.Products.Events;

/// <summary>
/// Event raised when a product's information is updated.
/// </summary>
public sealed record ProductUpdatedEvent(
    Guid ProductId,
    string Name,
    string Description,
    decimal Price) : DomainEvent;

