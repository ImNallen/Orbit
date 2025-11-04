using Domain.Abstractions;
using Domain.Products;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Products.Commands.UpdateProductStock;

/// <summary>
/// Handler for UpdateProductStockCommand.
/// </summary>
public sealed class UpdateProductStockCommandHandler
    : IRequestHandler<UpdateProductStockCommand, Result<UpdateProductStockResult, DomainError>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<UpdateProductStockCommandHandler> _logger;

    public UpdateProductStockCommandHandler(
        IProductRepository productRepository,
        ILogger<UpdateProductStockCommandHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<Result<UpdateProductStockResult, DomainError>> Handle(
        UpdateProductStockCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating stock for product {ProductId} to {Quantity}",
            command.ProductId, command.Quantity);

        // 1. Get product by ID
        Product? product = await _productRepository.GetByIdAsync(command.ProductId, cancellationToken);
        if (product is null)
        {
            _logger.LogWarning("Product {ProductId} not found", command.ProductId);
            return Result<UpdateProductStockResult, DomainError>.Failure(ProductErrors.ProductNotFound);
        }

        // 2. Update stock quantity
        Result<DomainError> updateResult = product.UpdateStock(command.Quantity);
        if (updateResult.IsFailure)
        {
            _logger.LogWarning("Failed to update stock for product {ProductId}: {Error}",
                command.ProductId, updateResult.Error.Message);
            return Result<UpdateProductStockResult, DomainError>.Failure(updateResult.Error);
        }

        // 3. Save changes
        await _productRepository.UpdateAsync(product, cancellationToken);

        _logger.LogInformation("Successfully updated stock for product {ProductId} to {Quantity}",
            command.ProductId, product.StockQuantity);

        return Result<UpdateProductStockResult, DomainError>.Success(
            new UpdateProductStockResult("Product stock updated successfully.", product.StockQuantity));
    }
}

