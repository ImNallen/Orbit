using Domain.Locations;
using Domain.Locations.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

/// <summary>
/// Repository implementation for Location aggregate.
/// </summary>
public class LocationRepository : ILocationRepository
{
    private readonly ApplicationDbContext _context;

    public LocationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Locations
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<List<Location>> GetAllAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default)
    {
        return await _context.Locations
            .AsNoTracking()
            .OrderBy(l => l.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Locations
            .AsNoTracking()
            .CountAsync(cancellationToken);
    }

    public async Task<(List<Location> Locations, int TotalCount)> QueryAsync(
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
        CancellationToken cancellationToken = default)
    {
        IQueryable<Location> query = _context.Locations.AsNoTracking();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(l =>
                EF.Functions.ILike(l.Name, $"%{searchTerm}%") ||
                EF.Functions.ILike(l.Address.City, $"%{searchTerm}%") ||
                EF.Functions.ILike(l.Address.State!, $"%{searchTerm}%"));
        }

        // Apply type filter
        if (type.HasValue)
        {
            query = query.Where(l => l.Type == type.Value);
        }

        // Apply status filter
        if (status.HasValue)
        {
            query = query.Where(l => l.Status == status.Value);
        }

        // Apply location filters
        if (!string.IsNullOrWhiteSpace(city))
        {
            query = query.Where(l => EF.Functions.ILike(l.Address.City, $"%{city}%"));
        }

        if (!string.IsNullOrWhiteSpace(state))
        {
            query = query.Where(l => EF.Functions.ILike(l.Address.State!, $"%{state}%"));
        }

        if (!string.IsNullOrWhiteSpace(country))
        {
            query = query.Where(l => EF.Functions.ILike(l.Address.Country, $"%{country}%"));
        }

        // Get total count before pagination
        int totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = sortBy.ToUpperInvariant() switch
        {
            "NAME" => sortDescending ? query.OrderByDescending(l => l.Name) : query.OrderBy(l => l.Name),
            "TYPE" => sortDescending ? query.OrderByDescending(l => l.Type) : query.OrderBy(l => l.Type),
            "STATUS" => sortDescending ? query.OrderByDescending(l => l.Status) : query.OrderBy(l => l.Status),
            "CITY" => sortDescending ? query.OrderByDescending(l => l.Address.City) : query.OrderBy(l => l.Address.City),
            "UPDATEDAT" => sortDescending ? query.OrderByDescending(l => l.UpdatedAt) : query.OrderBy(l => l.UpdatedAt),
            _ => sortDescending ? query.OrderByDescending(l => l.CreatedAt) : query.OrderBy(l => l.CreatedAt)
        };

        // Apply pagination
        List<Location> locations = await query
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return (locations, totalCount);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        string normalizedName = name.Trim();
        return await _context.Locations
            .AsNoTracking()
            .AnyAsync(l => l.Name == normalizedName, cancellationToken);
    }

    public async Task AddAsync(Location location, CancellationToken cancellationToken = default)
    {
        await _context.Locations.AddAsync(location, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Location location, CancellationToken cancellationToken = default)
    {
        _context.Locations.Update(location);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Location location, CancellationToken cancellationToken = default)
    {
        _context.Locations.Remove(location);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

