using System.Text.RegularExpressions;
using Domain.Abstractions;

namespace Domain.Products.ValueObjects;

/// <summary>
/// Value object representing a Stock Keeping Unit (SKU).
/// </summary>
public sealed partial record Sku
{
    private const int MinLength = 1;
    private const int MaxLength = 50;

    // SKU pattern: alphanumeric, hyphens, and underscores allowed
    private static readonly Regex SkuRegex = GenerateSkuRegex();

    private Sku(string value)
    {
        Value = value;
    }

    public string Value { get; }

    /// <summary>
    /// Creates a new Sku value object.
    /// </summary>
    /// <param name="sku">The SKU string.</param>
    /// <returns>Result containing the Sku or an error.</returns>
    public static Result<Sku, DomainError> Create(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
        {
            return Result<Sku, DomainError>.Failure(ProductErrors.InvalidSku);
        }

        // Normalize: trim and uppercase
        string normalizedSku = sku.Trim().ToUpperInvariant();

        if (normalizedSku.Length < MinLength)
        {
            return Result<Sku, DomainError>.Failure(ProductErrors.InvalidSku);
        }

        if (normalizedSku.Length > MaxLength)
        {
            return Result<Sku, DomainError>.Failure(ProductErrors.SkuTooLong);
        }

        if (!SkuRegex.IsMatch(normalizedSku))
        {
            return Result<Sku, DomainError>.Failure(ProductErrors.InvalidSkuFormat);
        }

        return Result<Sku, DomainError>.Success(new Sku(normalizedSku));
    }

    [GeneratedRegex(@"^[A-Z0-9\-_]+$", RegexOptions.Compiled)]
    private static partial Regex GenerateSkuRegex();

    public override string ToString() => Value;

    public static implicit operator string(Sku sku) => sku.Value;
}

