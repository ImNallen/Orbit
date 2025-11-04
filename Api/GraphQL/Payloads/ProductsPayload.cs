using Api.GraphQL.Types;

namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for products query with pagination.
/// </summary>
public sealed class ProductsPayload
{
    public IReadOnlyList<ProductSummaryType> Products { get; init; } = Array.Empty<ProductSummaryType>();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
    public IReadOnlyList<ProductError> Errors { get; init; } = Array.Empty<ProductError>();
}

