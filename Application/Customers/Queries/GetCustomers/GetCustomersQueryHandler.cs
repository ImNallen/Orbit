using Domain.Abstractions;
using Domain.Customers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Customers.Queries.GetCustomers;

/// <summary>
/// Handler for GetCustomersQuery.
/// </summary>
public sealed class GetCustomersQueryHandler
    : IRequestHandler<GetCustomersQuery, Result<GetCustomersResult, DomainError>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<GetCustomersQueryHandler> _logger;

    public GetCustomersQueryHandler(
        ICustomerRepository customerRepository,
        ILogger<GetCustomersQueryHandler> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<Result<GetCustomersResult, DomainError>> Handle(
        GetCustomersQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting customers - Page: {Page}, PageSize: {PageSize}, SearchTerm: {SearchTerm}, Status: {Status}",
            query.Page, query.PageSize, query.SearchTerm, query.Status);

        // 1. Validate pagination parameters
        if (query.Page < 1)
        {
            return Result<GetCustomersResult, DomainError>.Failure(ValidationErrors.InvalidPage);
        }

        if (query.PageSize < 1 || query.PageSize > 100)
        {
            return Result<GetCustomersResult, DomainError>.Failure(ValidationErrors.InvalidPageSize);
        }

        // 2. Calculate skip and take
        int skip = (query.Page - 1) * query.PageSize;
        int take = query.PageSize;

        // 3. Convert sort field enum to string
        string sortBy = query.SortBy.ToString();
        bool sortDescending = query.SortOrder == SortOrder.Descending;

        // 4. Query customers with filters, search, and sorting
        (List<Customer> customers, int totalCount) = await _customerRepository.QueryAsync(
            searchTerm: query.SearchTerm,
            status: query.Status,
            country: query.Country,
            state: query.State,
            city: query.City,
            createdAfter: query.CreatedAfter,
            createdBefore: query.CreatedBefore,
            sortBy: sortBy,
            sortDescending: sortDescending,
            skip: skip,
            take: take,
            cancellationToken: cancellationToken);

        // 5. Map to DTOs
        var customerDtos = customers.Select(c => new CustomerDto(
            c.Id,
            c.Email.Value,
            c.FullName.FirstName,
            c.FullName.LastName,
            c.PhoneNumber?.Value,
            c.Address.Street,
            c.Address.City,
            c.Address.State,
            c.Address.Country,
            c.Address.ZipCode,
            c.Status.ToString(),
            c.CreatedAt,
            c.UpdatedAt)).ToList();

        // 6. Calculate total pages
        int totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);

        _logger.LogDebug("Retrieved {CustomerCount} customers out of {TotalCount} total customers",
            customers.Count, totalCount);

        return Result<GetCustomersResult, DomainError>.Success(
            new GetCustomersResult(
                customerDtos,
                totalCount,
                query.Page,
                query.PageSize,
                totalPages));
    }
}

