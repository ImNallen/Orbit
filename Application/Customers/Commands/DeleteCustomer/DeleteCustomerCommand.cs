using Domain.Abstractions;
using MediatR;

namespace Application.Customers.Commands.DeleteCustomer;

/// <summary>
/// Command to soft delete a customer account.
/// </summary>
public sealed record DeleteCustomerCommand(Guid CustomerId) : IRequest<Result<DeleteCustomerResult, DomainError>>;

/// <summary>
/// Result of deleting a customer.
/// </summary>
public sealed record DeleteCustomerResult(string Message);

