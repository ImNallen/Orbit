using Domain.Abstractions;
using Domain.Customers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Customers.Commands.DeleteCustomer;

/// <summary>
/// Handler for DeleteCustomerCommand.
/// </summary>
public sealed class DeleteCustomerCommandHandler
    : IRequestHandler<DeleteCustomerCommand, Result<DeleteCustomerResult, DomainError>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<DeleteCustomerCommandHandler> _logger;

    public DeleteCustomerCommandHandler(
        ICustomerRepository customerRepository,
        ILogger<DeleteCustomerCommandHandler> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<Result<DeleteCustomerResult, DomainError>> Handle(
        DeleteCustomerCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting customer {CustomerId}", command.CustomerId);

        // 1. Get customer
        Customer? customer = await _customerRepository.GetByIdAsync(command.CustomerId, cancellationToken);
        if (customer is null)
        {
            _logger.LogWarning("Customer {CustomerId} not found", command.CustomerId);
            return Result<DeleteCustomerResult, DomainError>.Failure(
                CustomerErrors.CustomerNotFound);
        }

        // 2. Delete customer (soft delete)
        Result<DomainError> deleteResult = customer.Delete();
        if (deleteResult.IsFailure)
        {
            _logger.LogWarning("Failed to delete customer {CustomerId}", command.CustomerId);
            return Result<DeleteCustomerResult, DomainError>.Failure(deleteResult.Error);
        }

        // 3. Save changes
        await _customerRepository.UpdateAsync(customer, cancellationToken);

        _logger.LogInformation("Customer {CustomerId} deleted successfully", command.CustomerId);

        return Result<DeleteCustomerResult, DomainError>.Success(
            new DeleteCustomerResult($"Customer {customer.Email.Value} has been deleted"));
    }
}

