using Domain.Abstractions;
using Domain.Products;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Products.Commands.DeleteProduct;

/// <summary>
/// Handler for DeleteProductCommand.
/// </summary>
public sealed class DeleteProductCommandHandler
    : IRequestHandler<DeleteProductCommand, Result<DeleteProductResult, DomainError>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<DeleteProductCommandHandler> _logger;

    public DeleteProductCommandHandler(
        IProductRepository productRepository,
        ILogger<DeleteProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<Result<DeleteProductResult, DomainError>> Handle(
        DeleteProductCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting product {ProductId}", command.ProductId);

        // 1. Get product by ID
        Product? product = await _productRepository.GetByIdAsync(command.ProductId, cancellationToken);
        if (product is null)
        {
            _logger.LogWarning("Product {ProductId} not found", command.ProductId);
            return Result<DeleteProductResult, DomainError>.Failure(ProductErrors.ProductNotFound);
        }

        // 2. Delete product
        await _productRepository.DeleteAsync(product, cancellationToken);

        _logger.LogInformation("Successfully deleted product {ProductId}", command.ProductId);

        return Result<DeleteProductResult, DomainError>.Success(
            new DeleteProductResult("Product deleted successfully."));
    }
}

