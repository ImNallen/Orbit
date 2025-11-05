namespace Domain.Inventory;

/// <summary>
/// Repository interface for Inventory aggregate.
/// </summary>
public interface IInventoryRepository
{
    Task<Inventory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Inventory?> GetByProductAndLocationAsync(Guid productId, Guid locationId, CancellationToken cancellationToken = default);
    Task<List<Inventory>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<List<Inventory>> GetByLocationIdAsync(Guid locationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets inventory records for multiple locations (for location-based access control).
    /// </summary>
    Task<List<Inventory>> GetByLocationIdsAsync(IEnumerable<Guid> locationIds, CancellationToken cancellationToken = default);

    Task<List<Inventory>> GetAllAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total available quantity for a product across all locations.
    /// </summary>
    Task<int> GetTotalAvailableQuantityAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets inventory records with advanced filtering and sorting.
    /// </summary>
    Task<(List<Inventory> Inventories, int TotalCount)> QueryAsync(
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
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid productId, Guid locationId, CancellationToken cancellationToken = default);
    Task AddAsync(Inventory inventory, CancellationToken cancellationToken = default);
    Task UpdateAsync(Inventory inventory, CancellationToken cancellationToken = default);
    Task DeleteAsync(Inventory inventory, CancellationToken cancellationToken = default);
}

