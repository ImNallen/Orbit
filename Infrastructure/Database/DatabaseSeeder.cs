using Domain.Users.Permission;
using Domain.Users.Role;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Database;

/// <summary>
/// Seeds the database with initial data (roles, permissions, etc.).
/// </summary>
public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(ApplicationDbContext context, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Seeds all initial data.
    /// </summary>
    public async Task SeedAsync()
    {
        _logger.LogInformation("Starting database seeding...");

        await SeedPermissionsAsync();
        await SeedRolesAsync();

        _logger.LogInformation("Database seeding completed successfully.");
    }

    /// <summary>
    /// Seeds initial permissions.
    /// </summary>
    private async Task SeedPermissionsAsync()
    {
        // Check if permissions already exist
        if (await _context.Permissions.AnyAsync())
        {
            _logger.LogInformation("Permissions already exist. Skipping permission seeding.");
            return;
        }

        _logger.LogInformation("Seeding permissions...");

        Permission[] permissions = new[]
        {
            // User permissions
            Permission.Create("users:create", "Create new users", "users", "create"),
            Permission.Create("users:read", "Read user information", "users", "read"),
            Permission.Create("users:update", "Update user information", "users", "update"),
            Permission.Create("users:delete", "Delete users", "users", "delete"),
            
            // Role permissions
            Permission.Create("roles:create", "Create new roles", "roles", "create"),
            Permission.Create("roles:read", "Read role information", "roles", "read"),
            Permission.Create("roles:update", "Update role information", "roles", "update"),
            Permission.Create("roles:delete", "Delete roles", "roles", "delete"),
            Permission.Create("roles:assign", "Assign roles to users", "roles", "assign"),
            Permission.Create("roles:remove", "Remove roles from users", "roles", "remove"),
            
            // Permission permissions
            Permission.Create("permissions:read", "Read permission information", "permissions", "read"),
            
            // Session permissions
            Permission.Create("sessions:read", "Read session information", "sessions", "read"),
            Permission.Create("sessions:revoke", "Revoke sessions", "sessions", "revoke"),
        };

        await _context.Permissions.AddRangeAsync(permissions);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Seeded {Count} permissions.", permissions.Length);
    }

    /// <summary>
    /// Seeds initial roles.
    /// </summary>
    private async Task SeedRolesAsync()
    {
        // Check if roles already exist
        if (await _context.Roles.AnyAsync())
        {
            _logger.LogInformation("Roles already exist. Skipping role seeding.");
            return;
        }

        _logger.LogInformation("Seeding roles...");

        // Get all permissions
        List<Permission> allPermissions = await _context.Permissions.ToListAsync();

        // Admin role - has all permissions
        var adminPermissions = allPermissions.Select(p => p.Id).ToList();
        var adminRole = Role.Create("Admin", "Administrator with full system access");
        foreach (Guid permissionId in adminPermissions)
        {
            adminRole.AddPermission(permissionId);
        }

        // User role - has basic read permissions
        var userPermissions = allPermissions
            .Where(p => p.Action == "read")
            .Select(p => p.Id)
            .ToList();
        var userRole = Role.Create("User", "Standard user with read-only access");
        foreach (Guid permissionId in userPermissions)
        {
            userRole.AddPermission(permissionId);
        }

        await _context.Roles.AddRangeAsync(adminRole, userRole);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Seeded 2 roles (Admin, User).");
    }
}

