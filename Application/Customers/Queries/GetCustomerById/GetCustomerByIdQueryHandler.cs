using Domain.Abstractions;
using Domain.Customers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Customers.Queries.GetCustomerById;

/// <summary>
/// Handler for GetCustomerByIdQuery.
/// </summary>
public sealed class GetCustomerByIdQueryHandler
    : IRequestHandler<GetCustomerByIdQuery, Result<GetCustomerByIdResult, DomainError>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<GetCustomerByIdQueryHandler> _logger;

    public GetCustomerByIdQueryHandler(
        ICustomerRepository customerRepository,
        ILogger<GetCustomerByIdQueryHandler> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<Result<GetCustomerByIdResult, DomainError>> Handle(
        GetCustomerByIdQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Getting customer {CustomerId}", query.CustomerId);

        // 1. Get customer
        Customer? customer = await _customerRepository.GetByIdAsync(query.CustomerId, cancellationToken);
        if (customer is null)
        {
            _logger.LogWarning("Customer {CustomerId} not found", query.CustomerId);
            return Result<GetCustomerByIdResult, DomainError>.Failure(
                CustomerErrors.CustomerNotFound);
        }

        // 2. Return result
        return Result<GetCustomerByIdResult, DomainError>.Success(
            new GetCustomerByIdResult(
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

