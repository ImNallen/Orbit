using Api.GraphQL.Payloads;
using Api.GraphQL.Types;
using Application.Products.Queries.GetProductById;
using Application.Products.Queries.GetProductBySku;
using Application.Products.Queries.GetProducts;
using Domain.Abstractions;
using Domain.Products.Enums;
using HotChocolate.Authorization;
using MediatR;
using ProductSortField = Application.Products.Queries.GetProducts.ProductSortField;
using SortOrder = Application.Products.Queries.GetProducts.SortOrder;

namespace Api.GraphQL.Queries;

/// <summary>
/// GraphQL queries for products.
/// </summary>
[ExtendObjectType("Query")]
public sealed class ProductQueries
{
    /// <summary>
    /// Get all products with pagination, search, filtering, and sorting.
    /// Requires products:read permission.
    /// </summary>
    [Authorize(Policy = "products:read")]
    public async Task<ProductsPayload> ProductsAsync(
        [Service] IMediator mediator,
        int page = 1,
        int pageSize = 10,
        string? searchTerm = null,
        ProductStatusType? status = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? inStock = null,
        DateTime? createdAfter = null,
        DateTime? createdBefore = null,
        ProductSortFieldType sortBy = ProductSortFieldType.CreatedAt,
        SortOrderType sortOrder = SortOrderType.Descending,
        CancellationToken cancellationToken = default)
    {
        // Map GraphQL enums to Application layer enums
        ProductStatus? productStatus = status.HasValue
            ? Enum.Parse<ProductStatus>(status.Value.ToString())
            : null;

        ProductSortField productSortField = Enum.Parse<ProductSortField>(sortBy.ToString());
        SortOrder productSortOrder = Enum.Parse<SortOrder>(sortOrder.ToString());

        var query = new GetProductsQuery(
            page,
            pageSize,
            searchTerm,
            productStatus,
            minPrice,
            maxPrice,
            inStock,
            createdAfter,
            createdBefore,
            productSortField,
            productSortOrder);

        Result<GetProductsResult, DomainError> result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return new ProductsPayload
            {
                Errors = new[] { new ProductError(result.Error.Code, result.Error.Message) }
            };
        }

        GetProductsResult productsResult = result.Value;

        var productSummaries = productsResult.Products.Select(p => new ProductSummaryType
        {
            ProductId = p.ProductId,
            Name = p.Name,
            Price = p.Price,
            Sku = p.Sku,
            Status = p.Status
        }).ToList();

        return new ProductsPayload
        {
            Products = productSummaries,
            TotalCount = productsResult.TotalCount,
            Page = productsResult.Page,
            PageSize = productsResult.PageSize,
            TotalPages = productsResult.TotalPages
        };
    }

    /// <summary>
    /// Get a product by ID.
    /// Requires products:read permission.
    /// </summary>
    [Authorize(Policy = "products:read")]
    public async Task<ProductType?> ProductByIdAsync(
        Guid productId,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetProductByIdQuery(productId);
        Result<ProductDto, DomainError> result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return null;
        }

        ProductDto product = result.Value;

        return new ProductType
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Sku = product.Sku,
            Status = product.Status,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }

    /// <summary>
    /// Get a product by SKU.
    /// Requires products:read permission.
    /// </summary>
    [Authorize(Policy = "products:read")]
    public async Task<ProductType?> ProductBySkuAsync(
        string sku,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetProductBySkuQuery(sku);
        Result<ProductDto, DomainError> result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return null;
        }

        ProductDto product = result.Value;

        return new ProductType
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Sku = product.Sku,
            Status = product.Status,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}

