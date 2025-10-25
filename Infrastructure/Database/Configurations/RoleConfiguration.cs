using Domain.Users.Role;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

/// <summary>
/// Entity Framework configuration for Role entity.
/// </summary>
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(r => r.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.Description)
            .HasColumnName("description")
            .HasMaxLength(500)
            .IsRequired();

        // Permission IDs - stored as JSON array
        builder.Property(r => r.PermissionIds)
            .HasColumnName("permission_ids")
            .HasColumnType("jsonb") // PostgreSQL JSON type
            .IsRequired();

        // Unique index on role name
        builder.HasIndex(r => r.Name)
            .IsUnique()
            .HasDatabaseName("ix_roles_name");

        // Ignore domain events
        builder.Ignore(r => r.DomainEvents);
    }
}

