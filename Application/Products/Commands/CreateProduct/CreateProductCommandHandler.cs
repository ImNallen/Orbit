using Domain.Abstractions;
using Domain.Products;
using Domain.Products.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Products.Commands.CreateProduct;

/// <summary>
/// Handler for CreateProductCommand.
/// </summary>
public sealed class CreateProductCommandHandler
    : IRequestHandler<CreateProductCommand, Result<CreateProductResult, DomainError>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        ILogger<CreateProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<Result<CreateProductResult, DomainError>> Handle(
        CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new product with SKU {Sku}", command.Sku);

        // 1. Create value objects
        Result<ProductName, DomainError> nameResult = ProductName.Create(command.Name);
        if (nameResult.IsFailure)
        {
            _logger.LogWarning("Invalid product name: {Error}", nameResult.Error.Message);
            return Result<CreateProductResult, DomainError>.Failure(nameResult.Error);
        }

        Result<ProductDescription, DomainError> descriptionResult = ProductDescription.Create(command.Description);
        if (descriptionResult.IsFailure)
        {
            _logger.LogWarning("Invalid product description: {Error}", descriptionResult.Error.Message);
            return Result<CreateProductResult, DomainError>.Failure(descriptionResult.Error);
        }

        Result<Money, DomainError> priceResult = Money.Create(command.Price);
        if (priceResult.IsFailure)
        {
            _logger.LogWarning("Invalid product price: {Error}", priceResult.Error.Message);
            return Result<CreateProductResult, DomainError>.Failure(priceResult.Error);
        }

        Result<Sku, DomainError> skuResult = Sku.Create(command.Sku);
        if (skuResult.IsFailure)
        {
            _logger.LogWarning("Invalid product SKU: {Error}", skuResult.Error.Message);
            return Result<CreateProductResult, DomainError>.Failure(skuResult.Error);
        }

        // 2. Check if product with SKU already exists
        bool productExists = await _productRepository.ExistsBySkuAsync(skuResult.Value.Value, cancellationToken);
        if (productExists)
        {
            _logger.LogWarning("Product with SKU {Sku} already exists", skuResult.Value.Value);
            return Result<CreateProductResult, DomainError>.Failure(
                ProductErrors.SkuAlreadyExists);
        }

        // 3. Create Product entity
        Result<Product, DomainError> productResult = Product.Create(
            nameResult.Value,
            descriptionResult.Value,
            priceResult.Value,
            skuResult.Value);

        if (productResult.IsFailure)
        {
            _logger.LogWarning("Failed to create product entity: {Error}", productResult.Error.Message);
            return Result<CreateProductResult, DomainError>.Failure(productResult.Error);
        }

        Product product = productResult.Value;

        // Note: Initial stock is now managed via Inventory aggregate
        // Stock should be set by creating an Inventory record for the product at a location

        // 4. Save product to database
        await _productRepository.AddAsync(product, cancellationToken);

        _logger.LogInformation("Successfully created product {ProductId} with SKU {Sku}",
            product.Id, product.Sku.Value);

        return Result<CreateProductResult, DomainError>.Success(
            new CreateProductResult(
                product.Id,
                product.Name.Value,
                product.Sku.Value,
                product.Price.Amount));
    }
}

