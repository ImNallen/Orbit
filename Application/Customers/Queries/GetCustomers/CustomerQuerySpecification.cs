using Domain.Customers.Enums;

namespace Application.Customers.Queries.GetCustomers;

/// <summary>
/// Specification for querying customers with search, filtering, and sorting.
/// </summary>
public sealed record CustomerQuerySpecification(
    string? SearchTerm = null,
    CustomerStatus? Status = null,
    string? Country = null,
    string? State = null,
    string? City = null,
    DateTime? CreatedAfter = null,
    DateTime? CreatedBefore = null,
    CustomerSortField SortBy = CustomerSortField.CreatedAt,
    SortOrder SortOrder = SortOrder.Descending,
    int Skip = 0,
    int Take = 10);

