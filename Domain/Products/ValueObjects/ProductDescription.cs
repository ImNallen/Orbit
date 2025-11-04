using Domain.Abstractions;

namespace Domain.Products.ValueObjects;

/// <summary>
/// Value object representing a product description.
/// </summary>
public sealed record ProductDescription
{
    private const int MinLength = 1;
    private const int MaxLength = 2000;

    private ProductDescription(string value)
    {
        Value = value;
    }

    public string Value { get; }

    /// <summary>
    /// Creates a new ProductDescription value object.
    /// </summary>
    /// <param name="description">The product description.</param>
    /// <returns>Result containing the ProductDescription or an error.</returns>
    public static Result<ProductDescription, DomainError> Create(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return Result<ProductDescription, DomainError>.Failure(ProductErrors.InvalidProductDescription);
        }

        string trimmedDescription = description.Trim();

        if (trimmedDescription.Length < MinLength)
        {
            return Result<ProductDescription, DomainError>.Failure(ProductErrors.InvalidProductDescription);
        }

        if (trimmedDescription.Length > MaxLength)
        {
            return Result<ProductDescription, DomainError>.Failure(ProductErrors.ProductDescriptionTooLong);
        }

        return Result<ProductDescription, DomainError>.Success(new ProductDescription(trimmedDescription));
    }

    public override string ToString() => Value;

    public static implicit operator string(ProductDescription description) => description.Value;
}

