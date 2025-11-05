using Domain.Abstractions;
using Domain.Permission;
using Domain.Role;
using Domain.Shared.ValueObjects;
using Domain.Users;
using Domain.Users.ValueObjects;
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
    private readonly IPasswordHasher _passwordHasher;

    public DatabaseSeeder(
        ApplicationDbContext context,
        ILogger<DatabaseSeeder> logger,
        IPasswordHasher passwordHasher)
    {
        _context = context;
        _logger = logger;
        _passwordHasher = passwordHasher;
    }

    /// <summary>
    /// Seeds all initial data.
    /// </summary>
    public async Task SeedAsync()
    {
        _logger.LogInformation("Starting database seeding...");

        await SeedPermissionsAsync();
        await SeedRolesAsync();
        await SeedDefaultAdminUserAsync();

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

            // Location permissions
            Permission.Create("locations:create", "Create new locations", "locations", "create"),
            Permission.Create("locations:read", "Read location information", "locations", "read"),
            Permission.Create("locations:update", "Update location information", "locations", "update"),
            Permission.Create("locations:delete", "Delete locations", "locations", "delete"),

            // Inventory permissions
            Permission.Create("inventory:create", "Create inventory records", "inventory", "create"),
            Permission.Create("inventory:read", "Read inventory information", "inventory", "read"),
            Permission.Create("inventory:update", "Update inventory information", "inventory", "update"),
            Permission.Create("inventory:delete", "Delete inventory records", "inventory", "delete"),

            // Customer permissions
            Permission.Create("customers:create", "Create new customers", "customers", "create"),
            Permission.Create("customers:read", "Read customer information", "customers", "read"),
            Permission.Create("customers:update", "Update customer information", "customers", "update"),
            Permission.Create("customers:delete", "Delete customers", "customers", "delete"),

            // Product permissions
            Permission.Create("products:create", "Create new products", "products", "create"),
            Permission.Create("products:read", "Read product information", "products", "read"),
            Permission.Create("products:update", "Update product information", "products", "update"),
            Permission.Create("products:delete", "Delete products", "products", "delete"),
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

        // Helper function to get permission IDs by resource and actions
        List<Guid> GetPermissionIds(params string[] permissionNames) => allPermissions
                .Where(p => permissionNames.Contains(p.Name))
                .Select(p => p.Id)
                .ToList();

        // 1. HQ Admin Role - Full system access
        // Can manage everything. Needs to be assigned to all locations to access all data.
        var hqAdminRole = Role.Create(
            "HQ Admin",
            "Headquarters administrator with full system access");
        foreach (Guid permissionId in allPermissions.Select(p => p.Id))
        {
            hqAdminRole.AddPermission(permissionId);
        }

        // 2. Store Owner Role - Manages assigned locations
        // Can manage their assigned stores, view all data for assigned locations
        var storeOwnerRole = Role.Create(
            "Store Owner",
            "Owner of one or more store locations with management capabilities");
        List<Guid> storeOwnerPermissions = GetPermissionIds(
            // Location management for owned stores
            "locations:read",
            "locations:update",
            // User management for their stores
            "users:read",
            "users:create",
            "users:update",
            // Inventory management
            "inventory:read",
            "inventory:create",
            "inventory:update",
            // Product viewing (centralized catalog)
            "products:read",
            // Customer management
            "customers:read",
            "customers:create",
            "customers:update",
            // Session management
            "sessions:read"
        );
        foreach (Guid permissionId in storeOwnerPermissions)
        {
            storeOwnerRole.AddPermission(permissionId);
        }

        // 3. Store Manager Role - Manages assigned locations
        // Can manage their assigned stores, limited user management
        var storeManagerRole = Role.Create(
            "Store Manager",
            "Manager of store locations with operational capabilities");
        List<Guid> storeManagerPermissions = GetPermissionIds(
            // Location viewing
            "locations:read",
            // Limited user management (read only)
            "users:read",
            // Inventory management for their store
            "inventory:read",
            "inventory:create",
            "inventory:update",
            // Product viewing
            "products:read",
            // Customer management
            "customers:read",
            "customers:create",
            "customers:update",
            // Session management
            "sessions:read"
        );
        foreach (Guid permissionId in storeManagerPermissions)
        {
            storeManagerRole.AddPermission(permissionId);
        }

        // 4. Employee Role - Works at assigned locations
        // Basic operational access to assigned locations
        var employeeRole = Role.Create(
            "Employee",
            "Store employee with basic operational access to assigned locations");
        List<Guid> employeePermissions = GetPermissionIds(
            // Location viewing
            "locations:read",
            // User viewing (limited)
            "users:read",
            // Inventory viewing and basic updates
            "inventory:read",
            "inventory:update",
            // Product viewing
            "products:read",
            // Customer viewing and creation
            "customers:read",
            "customers:create",
            // Session viewing
            "sessions:read"
        );
        foreach (Guid permissionId in employeePermissions)
        {
            employeeRole.AddPermission(permissionId);
        }

        // 5. Read-Only User Role - Basic read access
        // For reporting, analytics, or limited access users
        var readOnlyRole = Role.Create(
            "Read-Only User",
            "User with read-only access to basic information");
        var readOnlyPermissions = allPermissions
            .Where(p => p.Action == "read")
            .Select(p => p.Id)
            .ToList();
        foreach (Guid permissionId in readOnlyPermissions)
        {
            readOnlyRole.AddPermission(permissionId);
        }

        await _context.Roles.AddRangeAsync(
            hqAdminRole,
            storeOwnerRole,
            storeManagerRole,
            employeeRole,
            readOnlyRole);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Seeded 5 roles (HQ Admin, Store Owner, Store Manager, Employee, Read-Only User).");
    }

    /// <summary>
    /// Seeds a default admin user for initial system access.
    /// </summary>
    private async Task SeedDefaultAdminUserAsync()
    {
        // Check if any users already exist
        if (await _context.Users.AnyAsync())
        {
            _logger.LogInformation("Users already exist. Skipping default admin user seeding.");
            return;
        }

        _logger.LogInformation("Seeding default admin user...");

        // Get the HQ Admin role
        Role? adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "HQ Admin");
        if (adminRole is null)
        {
            _logger.LogError("HQ Admin role not found. Cannot seed default admin user.");
            return;
        }

        // Create email
        Result<Email, DomainError> emailResult = Email.Create("admin@example.com");
        if (emailResult.IsFailure)
        {
            _logger.LogError("Failed to create admin email: {Error}", emailResult.Error.Message);
            return;
        }

        // Create password hash
        Result<Password, DomainError> passwordResult = Password.Create("Admin123!");
        if (passwordResult.IsFailure)
        {
            _logger.LogError("Failed to create admin password: {Error}", passwordResult.Error.Message);
            return;
        }

        PasswordHash passwordHash = _passwordHasher.Hash(passwordResult.Value);

        // Create full name
        Result<FullName, DomainError> fullNameResult = FullName.Create("System", "Administrator");
        if (fullNameResult.IsFailure)
        {
            _logger.LogError("Failed to create admin full name: {Error}", fullNameResult.Error.Message);
            return;
        }

        // Create admin user
        Result<User, DomainError> userResult = User.Create(
            emailResult.Value,
            passwordHash,
            fullNameResult.Value,
            adminRole.Id);

        if (userResult.IsFailure)
        {
            _logger.LogError("Failed to create admin user: {Error}", userResult.Error.Message);
            return;
        }

        User adminUser = userResult.Value;

        // Mark email as verified so admin can login immediately
        const string verificationToken = "SEEDED_ADMIN_TOKEN";
        adminUser.SetEmailVerificationToken(verificationToken, TimeSpan.FromHours(24));
        Result<DomainError> verifyResult = adminUser.VerifyEmail(verificationToken);

        if (verifyResult.IsFailure)
        {
            _logger.LogError("Failed to verify admin email: {Error}", verifyResult.Error.Message);
            return;
        }

        await _context.Users.AddAsync(adminUser);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Seeded default admin user (admin@example.com / Admin123!).");
    }
}

