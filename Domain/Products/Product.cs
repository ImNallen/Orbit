using Domain.Abstractions;
using Domain.Products.Enums;
using Domain.Products.Events;
using Domain.Products.ValueObjects;

namespace Domain.Products;

/// <summary>
/// Represents a product in the system.
/// </summary>
public sealed class Product : Entity
{
    private Product(
        Guid id,
        ProductName name,
        ProductDescription description,
        Money price,
        Sku sku)
        : base(id)
    {
        Name = name;
        Description = description;
        Price = price;
        Sku = sku;
        Status = ProductStatus.Active;
        StockQuantity = 0;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // EF Core constructor
    private Product() {}

    // Product Information
    public ProductName Name { get; private set; }

    public ProductDescription Description { get; private set; }

    public Money Price { get; private set; }

    public Sku Sku { get; private set; }

    // Inventory
    public int StockQuantity { get; private set; }

    // Product Status
    public ProductStatus Status { get; private set; }

    // Timestamps
    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="name">The product name.</param>
    /// <param name="description">The product description.</param>
    /// <param name="price">The product price.</param>
    /// <param name="sku">The product SKU (Stock Keeping Unit).</param>
    /// <returns>Result containing the Product or an error.</returns>
    public static Result<Product, DomainError> Create(
        ProductName name,
        ProductDescription description,
        Money price,
        Sku sku)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(description);
        ArgumentNullException.ThrowIfNull(price);
        ArgumentNullException.ThrowIfNull(sku);

        var product = new Product(
            Guid.CreateVersion7(),
            name,
            description,
            price,
            sku);

        product.Raise(new ProductCreatedEvent(
            product.Id,
            product.Name.Value,
            product.Sku.Value,
            product.Price.Amount));

        return Result<Product, DomainError>.Success(product);
    }

    /// <summary>
    /// Updates the product information.
    /// </summary>
    public Result<DomainError> UpdateInfo(ProductName name, ProductDescription description, Money price)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(description);
        ArgumentNullException.ThrowIfNull(price);

        Name = name;
        Description = description;
        Price = price;
        UpdatedAt = DateTime.UtcNow;

        Raise(new ProductUpdatedEvent(Id, Name.Value, Description.Value, Price.Amount));

        return Result<DomainError>.Success();
    }

    /// <summary>
    /// Updates the stock quantity.
    /// </summary>
    public Result<DomainError> UpdateStock(int quantity)
    {
        if (quantity < 0)
        {
            return Result<DomainError>.Failure(ProductErrors.InvalidStockQuantity);
        }

        StockQuantity = quantity;
        UpdatedAt = DateTime.UtcNow;

        Raise(new ProductStockUpdatedEvent(Id, StockQuantity));

        return Result<DomainError>.Success();
    }

    /// <summary>
    /// Deactivates the product.
    /// </summary>
    public Result<DomainError> Deactivate()
    {
        if (Status == ProductStatus.Inactive)
        {
            return Result<DomainError>.Failure(ProductErrors.ProductAlreadyInactive);
        }

        Status = ProductStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;

        return Result<DomainError>.Success();
    }

    /// <summary>
    /// Activates the product.
    /// </summary>
    public Result<DomainError> Activate()
    {
        if (Status == ProductStatus.Active)
        {
            return Result<DomainError>.Failure(ProductErrors.ProductAlreadyActive);
        }

        Status = ProductStatus.Active;
        UpdatedAt = DateTime.UtcNow;

        return Result<DomainError>.Success();
    }

    /// <summary>
    /// Checks if the product is available for purchase.
    /// </summary>
    public bool IsAvailable() => Status == ProductStatus.Active && StockQuantity > 0;
}
