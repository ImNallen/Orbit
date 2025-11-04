using Domain.Customers;
using Domain.Customers.Enums;
using Domain.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

/// <summary>
/// Repository implementation for Customer aggregate.
/// </summary>
public class CustomerRepository : ICustomerRepository
{
    private readonly ApplicationDbContext _context;

    public CustomerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Customer?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(c => c.Email.Value == email.Value, cancellationToken);
    }

    public async Task<List<Customer>> GetAllAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .OrderBy(c => c.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Customers.CountAsync(cancellationToken);
    }

    public async Task<(List<Customer> Customers, int TotalCount)> QueryAsync(
        string? searchTerm = null,
        CustomerStatus? status = null,
        string? country = null,
        string? state = null,
        string? city = null,
        DateTime? createdAfter = null,
        DateTime? createdBefore = null,
        string sortBy = "CreatedAt",
        bool sortDescending = true,
        int skip = 0,
        int take = 100,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Customer> query = _context.Customers;

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(c =>
                EF.Functions.ILike(c.Email.Value, $"%{searchTerm}%") ||
                EF.Functions.ILike(c.FullName.FirstName, $"%{searchTerm}%") ||
                EF.Functions.ILike(c.FullName.LastName, $"%{searchTerm}%") ||
                c.PhoneNumber != null && c.PhoneNumber.Value.Contains(searchTerm));
        }

        // Apply status filter
        if (status.HasValue)
        {
            query = query.Where(c => c.Status == status.Value);
        }

        // Apply country filter
        if (!string.IsNullOrWhiteSpace(country))
        {
            query = query.Where(c => EF.Functions.ILike(c.Address.Country, country));
        }

        // Apply state filter
        if (!string.IsNullOrWhiteSpace(state))
        {
            query = query.Where(c => EF.Functions.ILike(c.Address.State, state));
        }

        // Apply city filter
        if (!string.IsNullOrWhiteSpace(city))
        {
            query = query.Where(c => EF.Functions.ILike(c.Address.City, city));
        }

        // Apply date range filters
        if (createdAfter.HasValue)
        {
            query = query.Where(c => c.CreatedAt >= createdAfter.Value);
        }

        if (createdBefore.HasValue)
        {
            query = query.Where(c => c.CreatedAt <= createdBefore.Value);
        }

        // Get total count before pagination
        int totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = sortBy.ToUpperInvariant() switch
        {
            "EMAIL" => sortDescending
                ? query.OrderByDescending(c => c.Email.Value)
                : query.OrderBy(c => c.Email.Value),
            "FIRSTNAME" => sortDescending
                ? query.OrderByDescending(c => c.FullName.FirstName)
                : query.OrderBy(c => c.FullName.FirstName),
            "LASTNAME" => sortDescending
                ? query.OrderByDescending(c => c.FullName.LastName)
                : query.OrderBy(c => c.FullName.LastName),
            "UPDATEDAT" => sortDescending
                ? query.OrderByDescending(c => c.UpdatedAt)
                : query.OrderBy(c => c.UpdatedAt),
            _ => sortDescending
                ? query.OrderByDescending(c => c.CreatedAt)
                : query.OrderBy(c => c.CreatedAt)
        };

        // Apply pagination
        List<Customer> customers = await query
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return (customers, totalCount);
    }

    public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .AnyAsync(c => c.Email.Value == email.Value, cancellationToken);
    }

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        await _context.Customers.AddAsync(customer, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

