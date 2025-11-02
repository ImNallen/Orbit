using Domain.Abstractions;
using MediatR;

namespace Application.Customers.Commands.SuspendCustomer;

/// <summary>
/// Command to suspend a customer account.
/// </summary>
public sealed record SuspendCustomerCommand(Guid CustomerId) : IRequest<Result<SuspendCustomerResult, DomainError>>;

/// <summary>
/// Result of suspending a customer.
/// </summary>
public sealed record SuspendCustomerResult(string Message);

