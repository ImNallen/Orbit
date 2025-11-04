using Domain.Abstractions;
using Domain.Products;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Products.Commands.DeactivateProduct;

/// <summary>
/// Handler for DeactivateProductCommand.
/// </summary>
public sealed class DeactivateProductCommandHandler
    : IRequestHandler<DeactivateProductCommand, Result<DeactivateProductResult, DomainError>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<DeactivateProductCommandHandler> _logger;

    public DeactivateProductCommandHandler(
        IProductRepository productRepository,
        ILogger<DeactivateProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<Result<DeactivateProductResult, DomainError>> Handle(
        DeactivateProductCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deactivating product {ProductId}", command.ProductId);

        // 1. Get product by ID
        Product? product = await _productRepository.GetByIdAsync(command.ProductId, cancellationToken);
        if (product is null)
        {
            _logger.LogWarning("Product {ProductId} not found", command.ProductId);
            return Result<DeactivateProductResult, DomainError>.Failure(ProductErrors.ProductNotFound);
        }

        // 2. Deactivate product
        Result<DomainError> deactivateResult = product.Deactivate();
        if (deactivateResult.IsFailure)
        {
            _logger.LogWarning("Failed to deactivate product {ProductId}: {Error}",
                command.ProductId, deactivateResult.Error.Message);
            return Result<DeactivateProductResult, DomainError>.Failure(deactivateResult.Error);
        }

        // 3. Save changes
        await _productRepository.UpdateAsync(product, cancellationToken);

        _logger.LogInformation("Successfully deactivated product {ProductId}", command.ProductId);

        return Result<DeactivateProductResult, DomainError>.Success(
            new DeactivateProductResult("Product deactivated successfully."));
    }
}

