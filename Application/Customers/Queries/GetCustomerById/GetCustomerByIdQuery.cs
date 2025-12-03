using Domain.Abstractions;
using MediatR;

namespace Application.Customers.Queries.GetCustomerById;

/// <summary>
/// Query to get a customer by their ID.
/// </summary>
public sealed record GetCustomerByIdQuery(Guid CustomerId) : IRequest<Result<GetCustomerByIdResult, DomainError>>;

/// <summary>
/// Result of getting a customer by ID.
/// </summary>
public sealed record GetCustomerByIdResult(
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

