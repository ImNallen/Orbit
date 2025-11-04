using Domain.Abstractions;
using Domain.Products;
using Domain.Products.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Products.Commands.UpdateProduct;

/// <summary>
/// Handler for UpdateProductCommand.
/// </summary>
public sealed class UpdateProductCommandHandler
    : IRequestHandler<UpdateProductCommand, Result<UpdateProductResult, DomainError>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    public UpdateProductCommandHandler(
        IProductRepository productRepository,
        ILogger<UpdateProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<Result<UpdateProductResult, DomainError>> Handle(
        UpdateProductCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating product {ProductId}", command.ProductId);

        // 1. Get product by ID
        Product? product = await _productRepository.GetByIdAsync(command.ProductId, cancellationToken);
        if (product is null)
        {
            _logger.LogWarning("Product {ProductId} not found", command.ProductId);
            return Result<UpdateProductResult, DomainError>.Failure(ProductErrors.ProductNotFound);
        }

        // 2. Create value objects
        Result<ProductName, DomainError> nameResult = ProductName.Create(command.Name);
        if (nameResult.IsFailure)
        {
            _logger.LogWarning("Invalid product name: {Error}", nameResult.Error.Message);
            return Result<UpdateProductResult, DomainError>.Failure(nameResult.Error);
        }

        Result<ProductDescription, DomainError> descriptionResult = ProductDescription.Create(command.Description);
        if (descriptionResult.IsFailure)
        {
            _logger.LogWarning("Invalid product description: {Error}", descriptionResult.Error.Message);
            return Result<UpdateProductResult, DomainError>.Failure(descriptionResult.Error);
        }

        Result<Money, DomainError> priceResult = Money.Create(command.Price);
        if (priceResult.IsFailure)
        {
            _logger.LogWarning("Invalid product price: {Error}", priceResult.Error.Message);
            return Result<UpdateProductResult, DomainError>.Failure(priceResult.Error);
        }

        // 3. Update product information
        Result<DomainError> updateResult = product.UpdateInfo(
            nameResult.Value,
            descriptionResult.Value,
            priceResult.Value);

        if (updateResult.IsFailure)
        {
            _logger.LogWarning("Failed to update product {ProductId}: {Error}",
                command.ProductId, updateResult.Error.Message);
            return Result<UpdateProductResult, DomainError>.Failure(updateResult.Error);
        }

        // 4. Save changes
        await _productRepository.UpdateAsync(product, cancellationToken);

        _logger.LogInformation("Successfully updated product {ProductId}", command.ProductId);

        return Result<UpdateProductResult, DomainError>.Success(
            new UpdateProductResult("Product updated successfully."));
    }
}

