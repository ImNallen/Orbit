using Application.Products.Queries.GetProductById;
using Domain.Abstractions;
using Domain.Products;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Products.Queries.GetProductBySku;

/// <summary>
/// Handler for GetProductBySkuQuery.
/// </summary>
public sealed class GetProductBySkuQueryHandler
    : IRequestHandler<GetProductBySkuQuery, Result<ProductDto, DomainError>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<GetProductBySkuQueryHandler> _logger;

    public GetProductBySkuQueryHandler(
        IProductRepository productRepository,
        ILogger<GetProductBySkuQueryHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<Result<ProductDto, DomainError>> Handle(
        GetProductBySkuQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting product by SKU {Sku}", query.Sku);

        Product? product = await _productRepository.GetBySkuAsync(query.Sku, cancellationToken);
        if (product is null)
        {
            _logger.LogWarning("Product with SKU {Sku} not found", query.Sku);
            return Result<ProductDto, DomainError>.Failure(ProductErrors.ProductNotFound);
        }

        var productDto = new ProductDto(
            product.Id,
            product.Name.Value,
            product.Description.Value,
            product.Price.Amount,
            product.Sku.Value,
            product.StockQuantity,
            product.Status.ToString(),
            product.CreatedAt,
            product.UpdatedAt);

        return Result<ProductDto, DomainError>.Success(productDto);
    }
}

