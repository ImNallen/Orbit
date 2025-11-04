using Domain.Abstractions;
using Domain.Products;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Products.Queries.GetProductById;

/// <summary>
/// Handler for GetProductByIdQuery.
/// </summary>
public sealed class GetProductByIdQueryHandler
    : IRequestHandler<GetProductByIdQuery, Result<ProductDto, DomainError>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<GetProductByIdQueryHandler> _logger;

    public GetProductByIdQueryHandler(
        IProductRepository productRepository,
        ILogger<GetProductByIdQueryHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<Result<ProductDto, DomainError>> Handle(
        GetProductByIdQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting product by ID {ProductId}", query.ProductId);

        Product? product = await _productRepository.GetByIdAsync(query.ProductId, cancellationToken);
        if (product is null)
        {
            _logger.LogWarning("Product {ProductId} not found", query.ProductId);
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

