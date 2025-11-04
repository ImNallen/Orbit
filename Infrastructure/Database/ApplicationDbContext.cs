using Application.Abstractions.Events;
using Domain.Abstractions;
using Domain.Customers;
using Domain.Permission;
using Domain.Products;
using Domain.Role;
using Domain.Session;
using Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

/// <summary>
/// Application database context for Entity Framework Core.
/// Publishes domain events after saving changes.
/// </summary>
public class ApplicationDbContext : DbContext
{
    private readonly IPublisher _publisher;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IPublisher publisher)
        : base(options)
    {
        _publisher = publisher;
    }

    // DbSets for all aggregates
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<PasswordHistory> PasswordHistory => Set<PasswordHistory>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    /// <summary>
    /// Saves changes and publishes domain events.
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 1. Collect domain events from all tracked entities
        var domainEvents = ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                var events = entity.DomainEvents.ToList();
                entity.ClearDomainEvents();
                return events;
            })
            .ToList();

        // 2. Save changes to the database
        int result = await base.SaveChangesAsync(cancellationToken);

        // 3. Publish domain events after successful save
        // Wrap each domain event in a notification wrapper to keep Domain layer pure
        foreach (IDomainEvent domainEvent in domainEvents)
        {
            Type notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
            object? notification = Activator.CreateInstance(notificationType, domainEvent);

            if (notification is not null)
            {
                await _publisher.Publish(notification, cancellationToken);
            }
        }

        return result;
    }
}

