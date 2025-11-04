using Domain.Abstractions;

namespace Domain.Products.ValueObjects;

/// <summary>
/// Value object representing a product name.
/// </summary>
public sealed record ProductName
{
    private const int MinLength = 1;
    private const int MaxLength = 200;

    private ProductName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    /// <summary>
    /// Creates a new ProductName value object.
    /// </summary>
    /// <param name="name">The product name.</param>
    /// <returns>Result containing the ProductName or an error.</returns>
    public static Result<ProductName, DomainError> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<ProductName, DomainError>.Failure(ProductErrors.InvalidProductName);
        }

        string trimmedName = name.Trim();

        if (trimmedName.Length < MinLength)
        {
            return Result<ProductName, DomainError>.Failure(ProductErrors.InvalidProductName);
        }

        if (trimmedName.Length > MaxLength)
        {
            return Result<ProductName, DomainError>.Failure(ProductErrors.ProductNameTooLong);
        }

        return Result<ProductName, DomainError>.Success(new ProductName(trimmedName));
    }

    public override string ToString() => Value;

    public static implicit operator string(ProductName name) => name.Value;
}

