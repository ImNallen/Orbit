using Domain.Products.Enums;

namespace Domain.Products;

/// <summary>
/// Repository interface for Product aggregate.
/// </summary>
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<List<Product>> GetAllAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets products with advanced filtering, searching, and sorting.
    /// </summary>
    Task<(List<Product> Products, int TotalCount)> QueryAsync(
        string? searchTerm = null,
        ProductStatus? status = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? inStock = null,
        DateTime? createdAfter = null,
        DateTime? createdBefore = null,
        string sortBy = "CreatedAt",
        bool sortDescending = true,
        int skip = 0,
        int take = 100,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
    Task DeleteAsync(Product product, CancellationToken cancellationToken = default);
}

