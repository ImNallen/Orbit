namespace Application.Products.Queries.GetProducts;

/// <summary>
/// Fields available for sorting products.
/// </summary>
public enum ProductSortField
{
    /// <summary>
    /// Sort by product name.
    /// </summary>
    Name,

    /// <summary>
    /// Sort by price.
    /// </summary>
    Price,

    /// <summary>
    /// Sort by SKU.
    /// </summary>
    Sku,

    /// <summary>
    /// Sort by stock quantity.
    /// </summary>
    Stock,

    /// <summary>
    /// Sort by creation date.
    /// </summary>
    CreatedAt,

    /// <summary>
    /// Sort by last update date.
    /// </summary>
    UpdatedAt
}

