namespace Domain.Permission;

/// <summary>
/// Repository interface for Permission aggregate.
/// </summary>
public interface IPermissionRepository
{
    /// <summary>
    /// Gets a permission by its unique identifier.
    /// </summary>
    /// <param name="id">The permission's unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The permission if found, otherwise null.</returns>
    Task<Permission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a permission by its name.
    /// </summary>
    /// <param name="name">The permission name (e.g., "users:create").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The permission if found, otherwise null.</returns>
    Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets multiple permissions by their identifiers.
    /// </summary>
    /// <param name="permissionIds">Collection of permission identifiers.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of permissions.</returns>
    Task<List<Permission>> GetByIdsAsync(IEnumerable<Guid> permissionIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all permissions.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of all permissions.</returns>
    Task<List<Permission>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets permissions by resource.
    /// </summary>
    /// <param name="resource">The resource name (e.g., "users", "posts").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of permissions for the specified resource.</returns>
    Task<List<Permission>> GetByResourceAsync(string resource, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets permissions by resource and action.
    /// </summary>
    /// <param name="resource">The resource name.</param>
    /// <param name="action">The action name (e.g., "create", "read", "update", "delete").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The permission if found, otherwise null.</returns>
    Task<Permission?> GetByResourceAndActionAsync(string resource, string action, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a permission with the specified name exists.
    /// </summary>
    /// <param name="name">The permission name to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if a permission with the name exists, otherwise false.</returns>
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new permission to the repository.
    /// </summary>
    /// <param name="permission">The permission to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(Permission permission, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing permission.
    /// </summary>
    /// <param name="permission">The permission to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateAsync(Permission permission, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a permission from the repository.
    /// </summary>
    /// <param name="permission">The permission to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync(Permission permission, CancellationToken cancellationToken = default);
}

