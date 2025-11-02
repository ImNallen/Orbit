using Domain.Abstractions;
using Domain.Customers;
using Domain.Shared.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Customers.Queries.GetCustomerByEmail;

/// <summary>
/// Handler for GetCustomerByEmailQuery.
/// </summary>
public sealed class GetCustomerByEmailQueryHandler
    : IRequestHandler<GetCustomerByEmailQuery, Result<GetCustomerByEmailResult, DomainError>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<GetCustomerByEmailQueryHandler> _logger;

    public GetCustomerByEmailQueryHandler(
        ICustomerRepository customerRepository,
        ILogger<GetCustomerByEmailQueryHandler> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<Result<GetCustomerByEmailResult, DomainError>> Handle(
        GetCustomerByEmailQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Getting customer by email {Email}", query.Email);

        // 1. Create Email value object
        Result<Email, DomainError> emailResult = Email.Create(query.Email);
        if (emailResult.IsFailure)
        {
            _logger.LogWarning("Invalid email format: {Email}", query.Email);
            return Result<GetCustomerByEmailResult, DomainError>.Failure(emailResult.Error);
        }

        // 2. Get customer
        Customer? customer = await _customerRepository.GetByEmailAsync(emailResult.Value, cancellationToken);
        if (customer is null)
        {
            _logger.LogWarning("Customer with email {Email} not found", query.Email);
            return Result<GetCustomerByEmailResult, DomainError>.Failure(
                CustomerErrors.CustomerNotFound);
        }

        // 3. Return result
        return Result<GetCustomerByEmailResult, DomainError>.Success(
            new GetCustomerByEmailResult(
                customer.Id,
                customer.Email.Value,
                customer.FullName.FirstName,
                customer.FullName.LastName,
                customer.PhoneNumber?.Value,
                customer.Address.Street,
                customer.Address.City,
                customer.Address.State,
                customer.Address.Country,
                customer.Address.ZipCode,
                customer.Status.ToString(),
                customer.CreatedAt,
                customer.UpdatedAt));
    }
}

