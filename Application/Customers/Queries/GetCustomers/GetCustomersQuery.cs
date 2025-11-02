using Domain.Abstractions;
using Domain.Customers.Enums;
using MediatR;

namespace Application.Customers.Queries.GetCustomers;

/// <summary>
/// Query to get all customers with pagination, search, filtering, and sorting.
/// </summary>
public sealed record GetCustomersQuery(
    int Page = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    CustomerStatus? Status = null,
    string? Country = null,
    string? State = null,
    string? City = null,
    DateTime? CreatedAfter = null,
    DateTime? CreatedBefore = null,
    CustomerSortField SortBy = CustomerSortField.CreatedAt,
    SortOrder SortOrder = SortOrder.Descending) : IRequest<Result<GetCustomersResult, DomainError>>;

