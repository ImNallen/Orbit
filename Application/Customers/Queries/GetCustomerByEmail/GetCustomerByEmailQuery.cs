using Domain.Abstractions;
using MediatR;

namespace Application.Customers.Queries.GetCustomerByEmail;

/// <summary>
/// Query to get a customer by their email address.
/// </summary>
public sealed record GetCustomerByEmailQuery(string Email) : IRequest<Result<GetCustomerByEmailResult, DomainError>>;

/// <summary>
/// Result of getting a customer by email.
/// </summary>
public sealed record GetCustomerByEmailResult(
    Guid CustomerId,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string Street,
    string City,
    string? State,
    string Country,
    string ZipCode,
    string Status,
    DateTime CreatedAt,
    DateTime UpdatedAt);

