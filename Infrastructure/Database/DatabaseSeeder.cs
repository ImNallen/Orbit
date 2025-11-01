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

        // Get the Admin role
        Role? adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
        if (adminRole is null)
        {
            _logger.LogError("Admin role not found. Cannot seed default admin user.");
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

