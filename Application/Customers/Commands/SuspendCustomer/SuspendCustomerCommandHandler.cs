using Domain.Abstractions;
using Domain.Customers;
using Domain.Customers.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Customers.Commands.SuspendCustomer;

/// <summary>
/// Handler for SuspendCustomerCommand.
/// </summary>
public sealed class SuspendCustomerCommandHandler
    : IRequestHandler<SuspendCustomerCommand, Result<SuspendCustomerResult, DomainError>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<SuspendCustomerCommandHandler> _logger;

    public SuspendCustomerCommandHandler(
        ICustomerRepository customerRepository,
        ILogger<SuspendCustomerCommandHandler> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<Result<SuspendCustomerResult, DomainError>> Handle(
        SuspendCustomerCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Suspending customer {CustomerId}", command.CustomerId);

        // 1. Get customer
        Customer? customer = await _customerRepository.GetByIdAsync(command.CustomerId, cancellationToken);
        if (customer is null)
        {
            _logger.LogWarning("Customer {CustomerId} not found", command.CustomerId);
            return Result<SuspendCustomerResult, DomainError>.Failure(
                CustomerErrors.CustomerNotFound);
        }

        // 2. Check if deleted
        if (customer.Status == CustomerStatus.Deleted)
        {
            _logger.LogWarning("Cannot suspend deleted customer {CustomerId}", command.CustomerId);
            return Result<SuspendCustomerResult, DomainError>.Failure(CustomerErrors.CustomerDeleted);
        }

        // 3. Suspend customer
        Result<DomainError> suspendResult = customer.Suspend();
        if (suspendResult.IsFailure)
        {
            _logger.LogWarning("Failed to suspend customer {CustomerId}", command.CustomerId);
            return Result<SuspendCustomerResult, DomainError>.Failure(suspendResult.Error);
        }

        // 4. Save changes
        await _customerRepository.UpdateAsync(customer, cancellationToken);

        _logger.LogInformation("Customer {CustomerId} suspended successfully", command.CustomerId);

        return Result<SuspendCustomerResult, DomainError>.Success(
            new SuspendCustomerResult($"Customer {customer.Email.Value} has been suspended"));
    }
}

