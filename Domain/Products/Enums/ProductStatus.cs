namespace Domain.Products.Enums;

/// <summary>
/// Represents the status of a product.
/// </summary>
public enum ProductStatus
{
    /// <summary>
    /// Product is active and available for purchase.
    /// </summary>
    Active = 0,

    /// <summary>
    /// Product is inactive and not available for purchase.
    /// </summary>
    Inactive = 1,

    /// <summary>
    /// Product has been discontinued.
    /// </summary>
    Discontinued = 2,

    /// <summary>
    /// Product has been soft-deleted and cannot be used.
    /// </summary>
    Deleted = 3
}

