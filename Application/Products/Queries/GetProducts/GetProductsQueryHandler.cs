using Application.Products.Queries.GetProductById;
using Domain.Abstractions;
using Domain.Products;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Products.Queries.GetProducts;

/// <summary>
/// Handler for GetProductsQuery.
/// </summary>
public sealed class GetProductsQueryHandler
    : IRequestHandler<GetProductsQuery, Result<GetProductsResult, DomainError>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<GetProductsQueryHandler> _logger;

    public GetProductsQueryHandler(
        IProductRepository productRepository,
        ILogger<GetProductsQueryHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<Result<GetProductsResult, DomainError>> Handle(
        GetProductsQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting products - Page: {Page}, PageSize: {PageSize}, SearchTerm: {SearchTerm}, Status: {Status}",
            query.Page, query.PageSize, query.SearchTerm, query.Status);

        // 1. Validate pagination parameters
        if (query.Page < 1)
        {
            return Result<GetProductsResult, DomainError>.Failure(ValidationErrors.InvalidPage);
        }

        if (query.PageSize < 1 || query.PageSize > 100)
        {
            return Result<GetProductsResult, DomainError>.Failure(ValidationErrors.InvalidPageSize);
        }

        // 2. Calculate skip and take
        int skip = (query.Page - 1) * query.PageSize;
        int take = query.PageSize;

        // 3. Convert sort field enum to string
        string sortBy = query.SortBy.ToString();
        bool sortDescending = query.SortOrder == SortOrder.Descending;

        // 4. Query products with filters, search, and sorting
        (List<Product> products, int totalCount) = await _productRepository.QueryAsync(
            searchTerm: query.SearchTerm,
            status: query.Status,
            minPrice: query.MinPrice,
            maxPrice: query.MaxPrice,
            inStock: query.InStock,
            createdAfter: query.CreatedAfter,
            createdBefore: query.CreatedBefore,
            sortBy: sortBy,
            sortDescending: sortDescending,
            skip: skip,
            take: take,
            cancellationToken: cancellationToken);

        // 5. Map to DTOs
        var productDtos = products.Select(p => new ProductDto(
            p.Id,
            p.Name.Value,
            p.Description.Value,
            p.Price.Amount,
            p.Sku.Value,
            p.StockQuantity,
            p.Status.ToString(),
            p.CreatedAt,
            p.UpdatedAt)).ToList();

        // 6. Calculate total pages
        int totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);

        _logger.LogDebug("Retrieved {ProductCount} products out of {TotalCount} total products",
            products.Count, totalCount);

        return Result<GetProductsResult, DomainError>.Success(
            new GetProductsResult(
                productDtos,
                totalCount,
                query.Page,
                query.PageSize,
                totalPages));
    }
}

