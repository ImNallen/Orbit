using Domain.Abstractions;
using MediatR;

namespace Application.Customers.Commands.CreateCustomer;

/// <summary>
/// Command to create a new customer.
/// </summary>
public sealed record CreateCustomerCommand(
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode) : IRequest<Result<CreateCustomerResult, DomainError>>;

/// <summary>
/// Result of customer creation.
/// </summary>
public sealed record CreateCustomerResult(
    Guid CustomerId,
    string Email,
    string FirstName,
    string LastName);

