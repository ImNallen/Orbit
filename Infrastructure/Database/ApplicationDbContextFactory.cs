using MediatR;
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

        // Create a no-op publisher for design-time (migrations don't need event publishing)
        IPublisher publisher = new NoOpPublisher();

        return new ApplicationDbContext(optionsBuilder.Options, publisher);
    }

    /// <summary>
    /// No-op publisher for design-time context creation (migrations).
    /// </summary>
    private sealed class NoOpPublisher : IPublisher
    {
        public Task Publish(object notification, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            return Task.CompletedTask;
        }
    }
}

