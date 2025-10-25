using Domain.Abstractions;

namespace Domain.Users.Role;

/// <summary>
/// Represents a role that groups permissions together.
/// </summary>
public sealed class Role : Entity
{
    private readonly List<Guid> _permissionIds = [];

    private Role(Guid id, string name, string description)
        : base(id)
    {
        Name = name;
        Description = description;
    }

    // EF Core constructor
    private Role() { }

    /// <summary>
    /// Unique role name (e.g., "Admin", "User", "Moderator").
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Human-readable description of this role.
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Collection of permission IDs associated with this role.
    /// </summary>
    public IReadOnlyCollection<Guid> PermissionIds => _permissionIds.AsReadOnly();

    /// <summary>
    /// Creates a new Role.
    /// </summary>
    public static Role Create(string name, string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);

        return new Role(
            Guid.NewGuid(),
            name.Trim(),
            description.Trim());
    }

    /// <summary>
    /// Adds a permission to this role.
    /// </summary>
    public void AddPermission(Guid permissionId)
    {
        if (!_permissionIds.Contains(permissionId))
        {
            _permissionIds.Add(permissionId);
        }
    }

    /// <summary>
    /// Removes a permission from this role.
    /// </summary>
    public void RemovePermission(Guid permissionId)
    {
        _permissionIds.Remove(permissionId);
    }

    /// <summary>
    /// Checks if this role has a specific permission.
    /// </summary>
    public bool HasPermission(Guid permissionId)
    {
        return _permissionIds.Contains(permissionId);
    }
}

