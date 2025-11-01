using Domain.Abstractions;

namespace Domain.Permission;

/// <summary>
/// Represents a permission that can be granted to roles or users.
/// </summary>
public sealed class Permission : Entity
{
    private Permission(Guid id, string name, string description, string resource, string action)
        : base(id)
    {
        Name = name;
        Description = description;
        Resource = resource;
        Action = action;
    }

    // EF Core constructor
    private Permission() { }

    /// <summary>
    /// Unique permission name (e.g., "users:create", "posts:delete").
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Human-readable description of what this permission allows.
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// The resource this permission applies to (e.g., "users", "posts").
    /// </summary>
    public string Resource { get; private set; } = string.Empty;

    /// <summary>
    /// The action this permission allows (e.g., "create", "read", "update", "delete").
    /// </summary>
    public string Action { get; private set; } = string.Empty;

    /// <summary>
    /// Creates a new Permission.
    /// </summary>
#pragma warning disable CA1308 // Permission resources and actions are case-insensitive and lowercase is the standard
    public static Permission Create(string name, string description, string resource, string action)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentException.ThrowIfNullOrWhiteSpace(resource);
        ArgumentException.ThrowIfNullOrWhiteSpace(action);

        return new Permission(
            Guid.CreateVersion7(),
            name.Trim(),
            description.Trim(),
            resource.Trim().ToLowerInvariant(),
            action.Trim().ToLowerInvariant());
    }
#pragma warning restore CA1308

    /// <summary>
    /// Checks if this permission matches the given resource and action.
    /// </summary>
    public bool Matches(string resource, string action)
    {
        return Resource.Equals(resource, StringComparison.OrdinalIgnoreCase) &&
               Action.Equals(action, StringComparison.OrdinalIgnoreCase);
    }
}

