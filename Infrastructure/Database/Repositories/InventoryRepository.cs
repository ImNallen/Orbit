using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

/// <summary>
/// Repository implementation for Inventory aggregate.
/// </summary>
public class InventoryRepository : Domain.Inventory.IInventoryRepository
{
    private readonly ApplicationDbContext _context;

    public InventoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Domain.Inventory.Inventory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Inventory
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<Domain.Inventory.Inventory?> GetByProductAndLocationAsync(
        Guid productId,
        Guid locationId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Inventory
            .FirstOrDefaultAsync(i => i.ProductId == productId && i.LocationId == locationId, cancellationToken);
    }

    public async Task<List<Domain.Inventory.Inventory>> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Inventory
            .AsNoTracking()
            .Where(i => i.ProductId == productId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Domain.Inventory.Inventory>> GetByLocationIdAsync(
        Guid locationId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Inventory
            .AsNoTracking()
            .Where(i => i.LocationId == locationId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Domain.Inventory.Inventory>> GetAllAsync(
        int skip = 0,
        int take = 100,
        CancellationToken cancellationToken = default)
    {
        return await _context.Inventory
            .AsNoTracking()
            .OrderBy(i => i.UpdatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Inventory
            .AsNoTracking()
            .CountAsync(cancellationToken);
    }

    public async Task<int> GetTotalAvailableQuantityAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.Inventory
            .AsNoTracking()
            .Where(i => i.ProductId == productId)
            .SumAsync(i => i.AvailableQuantity, cancellationToken);
    }

    public async Task<(List<Domain.Inventory.Inventory> Inventories, int TotalCount)> QueryAsync(
        Guid? productId = null,
        Guid? locationId = null,
        bool? hasStock = null,
        bool? hasReservations = null,
        int? minQuantity = null,
        int? maxQuantity = null,
        string sortBy = "UpdatedAt",
        bool sortDescending = true,
        int skip = 0,
        int take = 100,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Domain.Inventory.Inventory> query = _context.Inventory.AsNoTracking();

        // Apply product filter
        if (productId.HasValue)
        {
            query = query.Where(i => i.ProductId == productId.Value);
        }

        // Apply location filter
        if (locationId.HasValue)
        {
            query = query.Where(i => i.LocationId == locationId.Value);
        }

        // Apply stock filter
        if (hasStock.HasValue)
        {
            query = hasStock.Value
                ? query.Where(i => i.Quantity > 0)
                : query.Where(i => i.Quantity == 0);
        }

        // Apply reservations filter
        if (hasReservations.HasValue)
        {
            query = hasReservations.Value
                ? query.Where(i => i.ReservedQuantity > 0)
                : query.Where(i => i.ReservedQuantity == 0);
        }

        // Apply quantity filters
        if (minQuantity.HasValue)
        {
            query = query.Where(i => i.Quantity >= minQuantity.Value);
        }

        if (maxQuantity.HasValue)
        {
            query = query.Where(i => i.Quantity <= maxQuantity.Value);
        }

        // Get total count before pagination
        int totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = sortBy.ToUpperInvariant() switch
        {
            "QUANTITY" => sortDescending ? query.OrderByDescending(i => i.Quantity) : query.OrderBy(i => i.Quantity),
            "RESERVEDQUANTITY" => sortDescending ? query.OrderByDescending(i => i.ReservedQuantity) : query.OrderBy(i => i.ReservedQuantity),
            "AVAILABLEQUANTITY" => sortDescending ? query.OrderByDescending(i => i.AvailableQuantity) : query.OrderBy(i => i.AvailableQuantity),
            "CREATEDAT" => sortDescending ? query.OrderByDescending(i => i.CreatedAt) : query.OrderBy(i => i.CreatedAt),
            _ => sortDescending ? query.OrderByDescending(i => i.UpdatedAt) : query.OrderBy(i => i.UpdatedAt)
        };

        // Apply pagination
        List<Domain.Inventory.Inventory> inventories = await query
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return (inventories, totalCount);
    }

    public async Task<bool> ExistsAsync(Guid productId, Guid locationId, CancellationToken cancellationToken = default)
    {
        return await _context.Inventory
            .AsNoTracking()
            .AnyAsync(i => i.ProductId == productId && i.LocationId == locationId, cancellationToken);
    }

    public async Task AddAsync(Domain.Inventory.Inventory inventory, CancellationToken cancellationToken = default)
    {
        await _context.Inventory.AddAsync(inventory, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Domain.Inventory.Inventory inventory, CancellationToken cancellationToken = default)
    {
        _context.Inventory.Update(inventory);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Domain.Inventory.Inventory inventory, CancellationToken cancellationToken = default)
    {
        _context.Inventory.Remove(inventory);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

