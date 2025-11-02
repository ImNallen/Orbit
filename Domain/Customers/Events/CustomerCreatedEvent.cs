using Domain.Abstractions;

namespace Domain.Customers.Events;

/// <summary>
/// Event raised when a new customer is created.
/// </summary>
public sealed record CustomerCreatedEvent(
    Guid CustomerId,
    string Email,
    string FirstName,
    string LastName) : DomainEvent;

