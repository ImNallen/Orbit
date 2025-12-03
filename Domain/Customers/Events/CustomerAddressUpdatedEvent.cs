using Domain.Abstractions;

namespace Domain.Customers.Events;

/// <summary>
/// Event raised when a customer's address is updated.
/// </summary>
public sealed record CustomerAddressUpdatedEvent(
    Guid CustomerId,
    string Street,
    string City,
    string? State,
    string Country,
    string ZipCode) : DomainEvent;

