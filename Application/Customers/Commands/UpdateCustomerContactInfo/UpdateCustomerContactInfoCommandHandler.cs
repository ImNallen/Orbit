using Domain.Abstractions;
using Domain.Customers;
using Domain.Shared.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Customers.Commands.UpdateCustomerContactInfo;

/// <summary>
/// Handler for UpdateCustomerContactInfoCommand.
/// </summary>
public sealed class UpdateCustomerContactInfoCommandHandler
    : IRequestHandler<UpdateCustomerContactInfoCommand, Result<UpdateCustomerContactInfoResult, DomainError>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<UpdateCustomerContactInfoCommandHandler> _logger;

    public UpdateCustomerContactInfoCommandHandler(
        ICustomerRepository customerRepository,
        ILogger<UpdateCustomerContactInfoCommandHandler> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<Result<UpdateCustomerContactInfoResult, DomainError>> Handle(
        UpdateCustomerContactInfoCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating contact info for customer {CustomerId}", command.CustomerId);

        // 1. Get customer
        Customer? customer = await _customerRepository.GetByIdAsync(command.CustomerId, cancellationToken);
        if (customer is null)
        {
            _logger.LogWarning("Customer {CustomerId} not found", command.CustomerId);
            return Result<UpdateCustomerContactInfoResult, DomainError>.Failure(
                CustomerErrors.CustomerNotFound);
        }

        // 2. Create Email value object
        Result<Email, DomainError> emailResult = Email.Create(command.Email);
        if (emailResult.IsFailure)
        {
            _logger.LogWarning("Invalid email format: {Email}", command.Email);
            return Result<UpdateCustomerContactInfoResult, DomainError>.Failure(emailResult.Error);
        }

        // 3. Check if email is being changed and if new email already exists
        if (customer.Email.Value != command.Email)
        {
            bool emailExists = await _customerRepository.ExistsByEmailAsync(emailResult.Value, cancellationToken);
            if (emailExists)
            {
                _logger.LogWarning("Email {Email} is already in use by another customer", command.Email);
                return Result<UpdateCustomerContactInfoResult, DomainError>.Failure(
                    CustomerErrors.EmailAlreadyExists);
            }
        }

        // 4. Create FullName value object
        Result<FullName, DomainError> fullNameResult = FullName.Create(command.FirstName, command.LastName);
        if (fullNameResult.IsFailure)
        {
            _logger.LogWarning("Invalid full name: {FirstName} {LastName}", command.FirstName, command.LastName);
            return Result<UpdateCustomerContactInfoResult, DomainError>.Failure(fullNameResult.Error);
        }

        // 5. Create PhoneNumber value object (optional)
        PhoneNumber? phoneNumber = null;
        if (!string.IsNullOrWhiteSpace(command.PhoneNumber))
        {
            Result<PhoneNumber, DomainError> phoneResult = PhoneNumber.Create(command.PhoneNumber);
            if (phoneResult.IsFailure)
            {
                _logger.LogWarning("Invalid phone number: {PhoneNumber}", command.PhoneNumber);
                return Result<UpdateCustomerContactInfoResult, DomainError>.Failure(phoneResult.Error);
            }
            phoneNumber = phoneResult.Value;
        }

        // 6. Update customer contact info
        Result<DomainError> updateResult = customer.UpdateContactInfo(
            emailResult.Value,
            fullNameResult.Value,
            phoneNumber);

        if (updateResult.IsFailure)
        {
            _logger.LogWarning("Failed to update customer {CustomerId} contact info", command.CustomerId);
            return Result<UpdateCustomerContactInfoResult, DomainError>.Failure(updateResult.Error);
        }

        // 7. Save changes
        await _customerRepository.UpdateAsync(customer, cancellationToken);

        _logger.LogInformation("Customer {CustomerId} contact info updated successfully", command.CustomerId);

        return Result<UpdateCustomerContactInfoResult, DomainError>.Success(
            new UpdateCustomerContactInfoResult($"Customer {customer.Email.Value} contact information has been updated"));
    }
}

