using Microsoft.AspNetCore.Authorization;

namespace Api.Authorization;

/// <summary>
/// Authorization requirement that checks if a user has a specific permission.
/// </summary>
public sealed class PermissionRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Gets the required permission.
    /// </summary>
    public string Permission { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionRequirement"/> class.
    /// </summary>
    /// <param name="permission">The required permission in the format "resource:action" (e.g., "users:create").</param>
    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}

