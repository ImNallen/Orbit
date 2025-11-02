using Domain.Abstractions;
using MediatR;

namespace Application.Customers.Commands.UpdateCustomerAddress;

/// <summary>
/// Command to update customer address.
/// </summary>
public sealed record UpdateCustomerAddressCommand(
    Guid CustomerId,
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode) : IRequest<Result<UpdateCustomerAddressResult, DomainError>>;

/// <summary>
/// Result of updating customer address.
/// </summary>
public sealed record UpdateCustomerAddressResult(string Message);

