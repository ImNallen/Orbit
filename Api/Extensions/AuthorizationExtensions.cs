using Microsoft.AspNetCore.Authorization;
using Api.Authorization;

namespace Api.Extensions;

/// <summary>
/// Extension methods for configuring authorization.
/// </summary>
public static class AuthorizationExtensions
{
    /// <summary>
    /// Adds permission-based authorization policies.
    /// Automatically creates a policy for each permission in the format "resource:action".
    /// </summary>
    public static AuthorizationOptions AddPermissionPolicies(this AuthorizationOptions options)
    {
        // Define all permissions that should have policies
        // This matches the permissions seeded in DatabaseSeeder.cs
        string[] permissions = new[]
        {
            // User permissions
            "users:create",
            "users:read",
            "users:update",
            "users:delete",

            // Role permissions
            "roles:create",
            "roles:read",
            "roles:update",
            "roles:delete",
            "roles:assign",
            "roles:remove",

            // Permission permissions
            "permissions:read",

            // Session permissions
            "sessions:read",
            "sessions:revoke",

            // Location permissions
            "locations:create",
            "locations:read",
            "locations:update",
            "locations:delete",

            // Inventory permissions
            "inventory:create",
            "inventory:read",
            "inventory:update",
            "inventory:delete",

            // Customer permissions
            "customers:create",
            "customers:read",
            "customers:update",
            "customers:delete",

            // Product permissions
            "products:create",
            "products:read",
            "products:update",
            "products:delete",
        };

        // Register a policy for each permission
        foreach (string permission in permissions)
        {
            options.AddPolicy(permission, policy =>
                policy.Requirements.Add(new PermissionRequirement(permission)));
        }

        return options;
    }

    /// <summary>
    /// Alternative approach: Adds permission-based authorization policies using a fluent builder.
    /// This provides better organization and readability.
    /// </summary>
    public static AuthorizationOptions AddPermissionPoliciesWithBuilder(this AuthorizationOptions options)
    {
        var builder = new PermissionPolicyBuilder(options);

        builder
            // User permissions
            .AddResource("users", "create", "read", "update", "delete")
            
            // Role permissions
            .AddResource("roles", "create", "read", "update", "delete", "assign", "remove")
            
            // Permission permissions
            .AddResource("permissions", "read")
            
            // Session permissions
            .AddResource("sessions", "read", "revoke")
            
            // Location permissions
            .AddResource("locations", "create", "read", "update", "delete")
            
            // Inventory permissions
            .AddResource("inventory", "create", "read", "update", "delete")
            
            // Customer permissions
            .AddResource("customers", "create", "read", "update", "delete")
            
            // Product permissions
            .AddResource("products", "create", "read", "update", "delete");

        return options;
    }
}

/// <summary>
/// Fluent builder for creating permission-based authorization policies.
/// </summary>
public class PermissionPolicyBuilder
{
    private readonly AuthorizationOptions _options;

    public PermissionPolicyBuilder(AuthorizationOptions options)
    {
        _options = options;
    }

    /// <summary>
    /// Adds policies for a resource with the specified actions.
    /// </summary>
    /// <param name="resource">The resource name (e.g., "users", "products")</param>
    /// <param name="actions">The actions (e.g., "create", "read", "update", "delete")</param>
    public PermissionPolicyBuilder AddResource(string resource, params string[] actions)
    {
        foreach (string action in actions)
        {
            string permission = $"{resource}:{action}";
            _options.AddPolicy(permission, policy =>
                policy.Requirements.Add(new PermissionRequirement(permission)));
        }

        return this;
    }

    /// <summary>
    /// Adds a single permission policy.
    /// </summary>
    public PermissionPolicyBuilder AddPermission(string permission)
    {
        _options.AddPolicy(permission, policy =>
            policy.Requirements.Add(new PermissionRequirement(permission)));

        return this;
    }

    /// <summary>
    /// Adds standard CRUD policies for a resource.
    /// </summary>
    public PermissionPolicyBuilder AddCrudResource(string resource)
    {
        return AddResource(resource, "create", "read", "update", "delete");
    }
}

