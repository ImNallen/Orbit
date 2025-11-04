namespace Api.GraphQL.Types;

/// <summary>
/// GraphQL type for product with full details.
/// </summary>
public sealed class ProductType
{
    public Guid ProductId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Sku { get; init; } = string.Empty;
    public int StockQuantity { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

/// <summary>
/// GraphQL type for product summary.
/// </summary>
public sealed class ProductSummaryType
{
    public Guid ProductId { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Sku { get; init; } = string.Empty;
    public int StockQuantity { get; init; }
    public string Status { get; init; } = string.Empty;
}

