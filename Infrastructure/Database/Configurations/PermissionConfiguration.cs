using Domain.Permission;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

/// <summary>
/// Entity Framework configuration for Permission entity.
/// </summary>
public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(p => p.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasColumnName("description")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(p => p.Resource)
            .HasColumnName("resource")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Action)
            .HasColumnName("action")
            .HasMaxLength(50)
            .IsRequired();

        // Unique index on permission name
        builder.HasIndex(p => p.Name)
            .IsUnique()
            .HasDatabaseName("ix_permissions_name");

        // Index on resource for efficient querying
        builder.HasIndex(p => p.Resource)
            .HasDatabaseName("ix_permissions_resource");

        // Unique index on resource + action combination
        builder.HasIndex(p => new { p.Resource, p.Action })
            .IsUnique()
            .HasDatabaseName("ix_permissions_resource_action");

        // Ignore domain events
        builder.Ignore(p => p.DomainEvents);
    }
}

