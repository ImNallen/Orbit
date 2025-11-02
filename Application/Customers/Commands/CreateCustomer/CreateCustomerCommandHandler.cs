using Domain.Abstractions;
using Domain.Customers;
using Domain.Shared.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Customers.Commands.CreateCustomer;

/// <summary>
/// Handler for CreateCustomerCommand.
/// </summary>
public sealed class CreateCustomerCommandHandler
    : IRequestHandler<CreateCustomerCommand, Result<CreateCustomerResult, DomainError>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<CreateCustomerCommandHandler> _logger;

    public CreateCustomerCommandHandler(
        ICustomerRepository customerRepository,
        ILogger<CreateCustomerCommandHandler> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<Result<CreateCustomerResult, DomainError>> Handle(
        CreateCustomerCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new customer with email {Email}", command.Email);

        // 1. Create Email value object
        Result<Email, DomainError> emailResult = Email.Create(command.Email);
        if (emailResult.IsFailure)
        {
            _logger.LogWarning("Invalid email format: {Email}", command.Email);
            return Result<CreateCustomerResult, DomainError>.Failure(emailResult.Error);
        }

        Email email = emailResult.Value;

        // 2. Check if customer already exists
        bool customerExists = await _customerRepository.ExistsByEmailAsync(email, cancellationToken);
        if (customerExists)
        {
            _logger.LogWarning("Customer with email {Email} already exists", command.Email);
            return Result<CreateCustomerResult, DomainError>.Failure(
                CustomerErrors.EmailAlreadyExists);
        }

        // 3. Create FullName value object
        Result<FullName, DomainError> fullNameResult = FullName.Create(command.FirstName, command.LastName);
        if (fullNameResult.IsFailure)
        {
            _logger.LogWarning("Invalid full name: {FirstName} {LastName}", command.FirstName, command.LastName);
            return Result<CreateCustomerResult, DomainError>.Failure(fullNameResult.Error);
        }

        // 4. Create PhoneNumber value object (optional)
        PhoneNumber? phoneNumber = null;
        if (!string.IsNullOrWhiteSpace(command.PhoneNumber))
        {
            Result<PhoneNumber, DomainError> phoneResult = PhoneNumber.Create(command.PhoneNumber);
            if (phoneResult.IsFailure)
            {
                _logger.LogWarning("Invalid phone number: {PhoneNumber}", command.PhoneNumber);
                return Result<CreateCustomerResult, DomainError>.Failure(phoneResult.Error);
            }
            phoneNumber = phoneResult.Value;
        }

        // 5. Create Address value object
        Result<Address, DomainError> addressResult = Address.Create(
            command.Street,
            command.City,
            command.State,
            command.Country,
            command.ZipCode);
        if (addressResult.IsFailure)
        {
            _logger.LogWarning("Invalid address");
            return Result<CreateCustomerResult, DomainError>.Failure(addressResult.Error);
        }

        // 6. Create Customer entity
        Result<Customer, DomainError> customerResult = Customer.Create(
            email,
            fullNameResult.Value,
            addressResult.Value);

        if (customerResult.IsFailure)
        {
            _logger.LogWarning("Failed to create customer entity");
            return Result<CreateCustomerResult, DomainError>.Failure(customerResult.Error);
        }

        Customer customer = customerResult.Value;

        // 7. Update phone number if provided
        if (phoneNumber is not null)
        {
            Result<DomainError> updateResult = customer.UpdateContactInfo(
                email,
                fullNameResult.Value,
                phoneNumber);

            if (updateResult.IsFailure)
            {
                _logger.LogWarning("Failed to update customer contact info with phone number");
                return Result<CreateCustomerResult, DomainError>.Failure(updateResult.Error);
            }
        }

        // 8. Save customer to database
        await _customerRepository.AddAsync(customer, cancellationToken);

        _logger.LogInformation("Customer created successfully with ID {CustomerId}", customer.Id);

        return Result<CreateCustomerResult, DomainError>.Success(
            new CreateCustomerResult(
                customer.Id,
                customer.Email.Value,
                customer.FullName.FirstName,
                customer.FullName.LastName));
    }
}

