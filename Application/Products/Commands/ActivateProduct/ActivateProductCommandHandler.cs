using Domain.Abstractions;
using Domain.Products;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Products.Commands.ActivateProduct;

/// <summary>
/// Handler for ActivateProductCommand.
/// </summary>
public sealed class ActivateProductCommandHandler
    : IRequestHandler<ActivateProductCommand, Result<ActivateProductResult, DomainError>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<ActivateProductCommandHandler> _logger;

    public ActivateProductCommandHandler(
        IProductRepository productRepository,
        ILogger<ActivateProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<Result<ActivateProductResult, DomainError>> Handle(
        ActivateProductCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Activating product {ProductId}", command.ProductId);

        // 1. Get product by ID
        Product? product = await _productRepository.GetByIdAsync(command.ProductId, cancellationToken);
        if (product is null)
        {
            _logger.LogWarning("Product {ProductId} not found", command.ProductId);
            return Result<ActivateProductResult, DomainError>.Failure(ProductErrors.ProductNotFound);
        }

        // 2. Activate product
        Result<DomainError> activateResult = product.Activate();
        if (activateResult.IsFailure)
        {
            _logger.LogWarning("Failed to activate product {ProductId}: {Error}",
                command.ProductId, activateResult.Error.Message);
            return Result<ActivateProductResult, DomainError>.Failure(activateResult.Error);
        }

        // 3. Save changes
        await _productRepository.UpdateAsync(product, cancellationToken);

        _logger.LogInformation("Successfully activated product {ProductId}", command.ProductId);

        return Result<ActivateProductResult, DomainError>.Success(
            new ActivateProductResult("Product activated successfully."));
    }
}

