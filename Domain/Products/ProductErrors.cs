using Domain.Abstractions;

namespace Domain.Products;

/// <summary>
/// Contains all product-related domain errors.
/// </summary>
public static class ProductErrors
{
    // Product Management Errors
    public static readonly DomainError ProductNotFound = new ProductError(
        "Product.NotFound",
        "Product not found.");

    public static readonly DomainError InvalidProductName = new ProductError(
        "Product.InvalidName",
        "Product name cannot be empty.");

    public static readonly DomainError ProductNameTooLong = new ProductError(
        "Product.NameTooLong",
        "Product name cannot exceed 200 characters.");

    public static readonly DomainError InvalidProductDescription = new ProductError(
        "Product.InvalidDescription",
        "Product description cannot be empty.");

    public static readonly DomainError ProductDescriptionTooLong = new ProductError(
        "Product.DescriptionTooLong",
        "Product description cannot exceed 2000 characters.");

    public static readonly DomainError InvalidPrice = new ProductError(
        "Product.InvalidPrice",
        "Product price must be greater than or equal to zero.");

    public static readonly DomainError PriceTooHigh = new ProductError(
        "Product.PriceTooHigh",
        "Product price cannot exceed 999,999,999.99.");

    public static readonly DomainError InvalidCurrency = new ProductError(
        "Product.InvalidCurrency",
        "Currency code cannot be empty.");

    public static readonly DomainError InvalidCurrencyFormat = new ProductError(
        "Product.InvalidCurrencyFormat",
        "Currency code must be a 3-letter ISO 4217 code.");

    public static readonly DomainError UnsupportedCurrency = new ProductError(
        "Product.UnsupportedCurrency",
        "The specified currency is not supported.");

    public static readonly DomainError InvalidSku = new ProductError(
        "Product.InvalidSku",
        "Product SKU cannot be empty.");

    public static readonly DomainError SkuTooLong = new ProductError(
        "Product.SkuTooLong",
        "Product SKU cannot exceed 50 characters.");

    public static readonly DomainError InvalidSkuFormat = new ProductError(
        "Product.InvalidSkuFormat",
        "Product SKU can only contain alphanumeric characters, hyphens, and underscores.");

    public static readonly DomainError SkuAlreadyExists = new ProductError(
        "Product.SkuAlreadyExists",
        "A product with this SKU already exists.");

    public static readonly DomainError InvalidStockQuantity = new ProductError(
        "Product.InvalidStockQuantity",
        "Stock quantity must be greater than or equal to zero.");

    // Status Management Errors
    public static readonly DomainError ProductAlreadyActive = new ProductError(
        "Product.AlreadyActive",
        "Product is already active.");

    public static readonly DomainError ProductAlreadyInactive = new ProductError(
        "Product.AlreadyInactive",
        "Product is already inactive.");

    public static readonly DomainError ProductNotAvailable = new ProductError(
        "Product.NotAvailable",
        "Product is not available for purchase.");

    public static readonly DomainError InsufficientStock = new ProductError(
        "Product.InsufficientStock",
        "Insufficient stock available.");
}

/// <summary>
/// Represents a product-specific domain error.
/// </summary>
internal sealed record ProductError(string Code, string Message) : DomainError(Code, Message);

