using Application.Products.Queries.GetProductById;

namespace Application.Products.Queries.GetProducts;

/// <summary>
/// Result of getting products with pagination.
/// </summary>
public sealed record GetProductsResult(
    IReadOnlyList<ProductDto> Products,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages);

