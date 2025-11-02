using Domain.Abstractions;

namespace Domain.Customers.Events;

/// <summary>
/// Event raised when a customer's contact information is updated.
/// </summary>
public sealed record CustomerContactInfoUpdatedEvent(
    Guid CustomerId,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber) : DomainEvent;

