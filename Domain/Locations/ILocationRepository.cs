using Domain.Locations.Enums;

namespace Domain.Locations;

/// <summary>
/// Repository interface for Location aggregate.
/// </summary>
public interface ILocationRepository
{
    Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Location>> GetAllAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets locations with advanced filtering and sorting.
    /// </summary>
    Task<(List<Location> Locations, int TotalCount)> QueryAsync(
        string? searchTerm = null,
        LocationType? type = null,
        LocationStatus? status = null,
        string? city = null,
        string? state = null,
        string? country = null,
        string sortBy = "CreatedAt",
        bool sortDescending = true,
        int skip = 0,
        int take = 100,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    Task AddAsync(Location location, CancellationToken cancellationToken = default);
    Task UpdateAsync(Location location, CancellationToken cancellationToken = default);
    Task DeleteAsync(Location location, CancellationToken cancellationToken = default);
}

