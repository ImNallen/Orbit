using Domain.Abstractions;
using Domain.Customers;
using Domain.Shared.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Customers.Commands.UpdateCustomerAddress;

/// <summary>
/// Handler for UpdateCustomerAddressCommand.
/// </summary>
public sealed class UpdateCustomerAddressCommandHandler
    : IRequestHandler<UpdateCustomerAddressCommand, Result<UpdateCustomerAddressResult, DomainError>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<UpdateCustomerAddressCommandHandler> _logger;

    public UpdateCustomerAddressCommandHandler(
        ICustomerRepository customerRepository,
        ILogger<UpdateCustomerAddressCommandHandler> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<Result<UpdateCustomerAddressResult, DomainError>> Handle(
        UpdateCustomerAddressCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating address for customer {CustomerId}", command.CustomerId);

        // 1. Get customer
        Customer? customer = await _customerRepository.GetByIdAsync(command.CustomerId, cancellationToken);
        if (customer is null)
        {
            _logger.LogWarning("Customer {CustomerId} not found", command.CustomerId);
            return Result<UpdateCustomerAddressResult, DomainError>.Failure(
                CustomerErrors.CustomerNotFound);
        }

        // 2. Create Address value object
        Result<Address, DomainError> addressResult = Address.Create(
            command.Street,
            command.City,
            command.State,
            command.Country,
            command.ZipCode);

        if (addressResult.IsFailure)
        {
            _logger.LogWarning("Invalid address for customer {CustomerId}", command.CustomerId);
            return Result<UpdateCustomerAddressResult, DomainError>.Failure(addressResult.Error);
        }

        // 3. Update customer address
        Result<DomainError> updateResult = customer.UpdateAddress(addressResult.Value);

        if (updateResult.IsFailure)
        {
            _logger.LogWarning("Failed to update customer {CustomerId} address", command.CustomerId);
            return Result<UpdateCustomerAddressResult, DomainError>.Failure(updateResult.Error);
        }

        // 4. Save changes
        await _customerRepository.UpdateAsync(customer, cancellationToken);

        _logger.LogInformation("Customer {CustomerId} address updated successfully", command.CustomerId);

        return Result<UpdateCustomerAddressResult, DomainError>.Success(
            new UpdateCustomerAddressResult($"Customer {customer.Email.Value} address has been updated"));
    }
}

