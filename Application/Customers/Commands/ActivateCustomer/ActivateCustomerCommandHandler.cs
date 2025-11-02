using Domain.Abstractions;
using Domain.Customers;
using Domain.Customers.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Customers.Commands.ActivateCustomer;

/// <summary>
/// Handler for ActivateCustomerCommand.
/// </summary>
public sealed class ActivateCustomerCommandHandler
    : IRequestHandler<ActivateCustomerCommand, Result<ActivateCustomerResult, DomainError>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<ActivateCustomerCommandHandler> _logger;

    public ActivateCustomerCommandHandler(
        ICustomerRepository customerRepository,
        ILogger<ActivateCustomerCommandHandler> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<Result<ActivateCustomerResult, DomainError>> Handle(
        ActivateCustomerCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Activating customer {CustomerId}", command.CustomerId);

        // 1. Get customer
        Customer? customer = await _customerRepository.GetByIdAsync(command.CustomerId, cancellationToken);
        if (customer is null)
        {
            _logger.LogWarning("Customer {CustomerId} not found", command.CustomerId);
            return Result<ActivateCustomerResult, DomainError>.Failure(
                CustomerErrors.CustomerNotFound);
        }

        // 2. Check if deleted
        if (customer.Status == CustomerStatus.Deleted)
        {
            _logger.LogWarning("Cannot activate deleted customer {CustomerId}", command.CustomerId);
            return Result<ActivateCustomerResult, DomainError>.Failure(CustomerErrors.CustomerDeleted);
        }

        // 3. Activate customer
        Result<DomainError> activateResult = customer.Activate();
        if (activateResult.IsFailure)
        {
            _logger.LogWarning("Failed to activate customer {CustomerId}", command.CustomerId);
            return Result<ActivateCustomerResult, DomainError>.Failure(activateResult.Error);
        }

        // 4. Save changes
        await _customerRepository.UpdateAsync(customer, cancellationToken);

        _logger.LogInformation("Customer {CustomerId} activated successfully", command.CustomerId);

        return Result<ActivateCustomerResult, DomainError>.Success(
            new ActivateCustomerResult($"Customer {customer.Email.Value} has been activated"));
    }
}

