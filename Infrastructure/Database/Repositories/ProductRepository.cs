using Domain.Products;
using Domain.Products.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

/// <summary>
/// Repository implementation for Product aggregate.
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        string normalizedSku = sku.ToUpperInvariant();
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Sku == normalizedSku, cancellationToken);
    }

    public async Task<List<Product>> GetAllAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .OrderBy(p => p.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products.CountAsync(cancellationToken);
    }

    public async Task<(List<Product> Products, int TotalCount)> QueryAsync(
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
        CancellationToken cancellationToken = default)
    {
        IQueryable<Product> query = _context.Products;

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p =>
                EF.Functions.ILike(p.Name, $"%{searchTerm}%") ||
                EF.Functions.ILike(p.Description, $"%{searchTerm}%") ||
                EF.Functions.ILike(p.Sku, $"%{searchTerm}%"));
        }

        // Apply status filter
        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        // Apply price filters
        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        // Apply stock filter
        if (inStock.HasValue)
        {
            query = inStock.Value
                ? query.Where(p => p.StockQuantity > 0)
                : query.Where(p => p.StockQuantity == 0);
        }

        // Apply date filters
        if (createdAfter.HasValue)
        {
            query = query.Where(p => p.CreatedAt >= createdAfter.Value);
        }

        if (createdBefore.HasValue)
        {
            query = query.Where(p => p.CreatedAt <= createdBefore.Value);
        }

        // Get total count before pagination
        int totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = sortBy.ToUpperInvariant() switch
        {
            "NAME" => sortDescending
                ? query.OrderByDescending(p => p.Name)
                : query.OrderBy(p => p.Name),
            "PRICE" => sortDescending
                ? query.OrderByDescending(p => p.Price)
                : query.OrderBy(p => p.Price),
            "SKU" => sortDescending
                ? query.OrderByDescending(p => p.Sku)
                : query.OrderBy(p => p.Sku),
            "STOCK" => sortDescending
                ? query.OrderByDescending(p => p.StockQuantity)
                : query.OrderBy(p => p.StockQuantity),
            "CREATEDAT" => sortDescending
                ? query.OrderByDescending(p => p.CreatedAt)
                : query.OrderBy(p => p.CreatedAt),
            _ => sortDescending
                ? query.OrderByDescending(p => p.CreatedAt)
                : query.OrderBy(p => p.CreatedAt)
        };

        // Apply pagination
        List<Product> products = await query
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return (products, totalCount);
    }

    public async Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        string normalizedSku = sku.ToUpperInvariant();
        return await _context.Products
            .AnyAsync(p => p.Sku == normalizedSku, cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(product, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Product product, CancellationToken cancellationToken = default)
    {
        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

