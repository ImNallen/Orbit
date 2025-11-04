using Domain.Abstractions;
using Domain.Products.Enums;
using MediatR;

namespace Application.Products.Queries.GetProducts;

/// <summary>
/// Query to get all products with pagination, search, filtering, and sorting.
/// </summary>
public sealed record GetProductsQuery(
    int Page = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    ProductStatus? Status = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    bool? InStock = null,
    DateTime? CreatedAfter = null,
    DateTime? CreatedBefore = null,
    ProductSortField SortBy = ProductSortField.CreatedAt,
    SortOrder SortOrder = SortOrder.Descending) : IRequest<Result<GetProductsResult, DomainError>>;

