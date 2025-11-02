using Api.GraphQL.Payloads;
using Api.GraphQL.Types;
using Application.Customers.Queries.GetCustomerById;
using Application.Customers.Queries.GetCustomers;
using Application.Customers.Queries.GetCustomerByEmail;
using Domain.Abstractions;
using Domain.Customers.Enums;
using HotChocolate.Authorization;
using MediatR;

namespace Api.GraphQL.Queries;

/// <summary>
/// GraphQL queries for customers.
/// </summary>
[ExtendObjectType("Query")]
public sealed class CustomerQueries
{
    /// <summary>
    /// Get all customers with pagination, search, filtering, and sorting.
    /// Requires customers:read permission.
    /// </summary>
    [Authorize(Policy = "customers:read")]
    public async Task<CustomersPayload> CustomersAsync(
        [Service] IMediator mediator,
        int page = 1,
        int pageSize = 10,
        string? searchTerm = null,
        CustomerStatusType? status = null,
        string? country = null,
        string? state = null,
        string? city = null,
        DateTime? createdAfter = null,
        DateTime? createdBefore = null,
        CustomerSortFieldType sortBy = CustomerSortFieldType.CreatedAt,
        SortOrderType sortOrder = SortOrderType.Descending,
        CancellationToken cancellationToken = default)
    {
        // Map GraphQL enums to domain enums
        CustomerStatus? domainStatus = status.HasValue
            ? Enum.Parse<CustomerStatus>(status.Value.ToString())
            : null;

        CustomerSortField domainSortBy = Enum.Parse<CustomerSortField>(sortBy.ToString());
        SortOrder domainSortOrder = Enum.Parse<SortOrder>(sortOrder.ToString());

        var query = new GetCustomersQuery(
            page,
            pageSize,
            searchTerm,
            domainStatus,
            country,
            state,
            city,
            createdAfter,
            createdBefore,
            domainSortBy,
            domainSortOrder);

        Result<GetCustomersResult, DomainError> result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return new CustomersPayload
            {
                Customers = Array.Empty<CustomerSummaryType>(),
                TotalCount = 0,
                Page = page,
                PageSize = pageSize,
                TotalPages = 0,
                Errors = new[] { new CustomerError(result.Error.Code, result.Error.Message) }
            };
        }

        var customers = result.Value.Customers.Select(c => new CustomerSummaryType
        {
            CustomerId = c.CustomerId,
            Email = c.Email,
            FirstName = c.FirstName,
            LastName = c.LastName,
            PhoneNumber = c.PhoneNumber,
            Status = c.Status,
            CreatedAt = c.CreatedAt
        }).ToList();

        return new CustomersPayload
        {
            Customers = customers,
            TotalCount = result.Value.TotalCount,
            Page = result.Value.Page,
            PageSize = result.Value.PageSize,
            TotalPages = result.Value.TotalPages
        };
    }

    /// <summary>
    /// Get a customer by their ID.
    /// Requires customers:read permission.
    /// </summary>
    [Authorize(Policy = "customers:read")]
    public async Task<CustomerType?> CustomerAsync(
        Guid id,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetCustomerByIdQuery(id);
        Result<GetCustomerByIdResult, DomainError> result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return null;
        }

        return new CustomerType
        {
            CustomerId = result.Value.CustomerId,
            Email = result.Value.Email,
            FirstName = result.Value.FirstName,
            LastName = result.Value.LastName,
            PhoneNumber = result.Value.PhoneNumber,
            Address = new AddressType
            {
                Street = result.Value.Street,
                City = result.Value.City,
                State = result.Value.State,
                Country = result.Value.Country,
                ZipCode = result.Value.ZipCode
            },
            Status = result.Value.Status,
            CreatedAt = result.Value.CreatedAt,
            UpdatedAt = result.Value.UpdatedAt
        };
    }

    /// <summary>
    /// Get a customer by their email address.
    /// Requires customers:read permission.
    /// </summary>
    [Authorize(Policy = "customers:read")]
    public async Task<CustomerType?> CustomerByEmailAsync(
        string email,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetCustomerByEmailQuery(email);
        Result<GetCustomerByEmailResult, DomainError> result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return null;
        }

        return new CustomerType
        {
            CustomerId = result.Value.CustomerId,
            Email = result.Value.Email,
            FirstName = result.Value.FirstName,
            LastName = result.Value.LastName,
            PhoneNumber = result.Value.PhoneNumber,
            Address = new AddressType
            {
                Street = result.Value.Street,
                City = result.Value.City,
                State = result.Value.State,
                Country = result.Value.Country,
                ZipCode = result.Value.ZipCode
            },
            Status = result.Value.Status,
            CreatedAt = result.Value.CreatedAt,
            UpdatedAt = result.Value.UpdatedAt
        };
    }
}

