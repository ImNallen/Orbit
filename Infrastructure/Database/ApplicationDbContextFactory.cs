using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Database;

/// <summary>
/// Design-time factory for creating ApplicationDbContext instances for EF Core migrations.
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        // Use a default connection string for migrations
        // This will be overridden at runtime by the actual configuration
        optionsBuilder.UseNpgsql("Host=localhost;Database=orbit;Username=postgres;Password=postgres");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}

