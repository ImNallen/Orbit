using Domain.Abstractions;
using MediatR;

namespace Application.Customers.Commands.ActivateCustomer;

/// <summary>
/// Command to activate a suspended customer account.
/// </summary>
public sealed record ActivateCustomerCommand(Guid CustomerId) : IRequest<Result<ActivateCustomerResult, DomainError>>;

/// <summary>
/// Result of activating a customer.
/// </summary>
public sealed record ActivateCustomerResult(string Message);

