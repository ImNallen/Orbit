using Domain.Customers.Enums;
using Domain.Shared.ValueObjects;

namespace Domain.Customers;

/// <summary>
/// Repository interface for Customer aggregate.
/// </summary>
public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Customer?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<List<Customer>> GetAllAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets customers with advanced filtering, searching, and sorting.
    /// </summary>
    Task<(List<Customer> Customers, int TotalCount)> QueryAsync(
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
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
    Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default);
    Task DeleteAsync(Customer customer, CancellationToken cancellationToken = default);
}
