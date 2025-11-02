using Domain.Abstractions;
using MediatR;

namespace Application.Customers.Commands.UpdateCustomerContactInfo;

/// <summary>
/// Command to update customer contact information.
/// </summary>
public sealed record UpdateCustomerContactInfoCommand(
    Guid CustomerId,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber) : IRequest<Result<UpdateCustomerContactInfoResult, DomainError>>;

/// <summary>
/// Result of updating customer contact information.
/// </summary>
public sealed record UpdateCustomerContactInfoResult(string Message);

