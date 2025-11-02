using Api.GraphQL.Types;

namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for customers query with pagination.
/// </summary>
public sealed class CustomersPayload
{
    public IReadOnlyList<CustomerSummaryType> Customers { get; init; } = Array.Empty<CustomerSummaryType>();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
    public IReadOnlyList<CustomerError> Errors { get; init; } = Array.Empty<CustomerError>();
}

