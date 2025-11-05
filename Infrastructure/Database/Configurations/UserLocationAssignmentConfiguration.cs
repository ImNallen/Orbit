using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

/// <summary>
/// Entity Framework configuration for UserLocationAssignment entity.
/// </summary>
public class UserLocationAssignmentConfiguration : IEntityTypeConfiguration<UserLocationAssignment>
{
    public void Configure(EntityTypeBuilder<UserLocationAssignment> builder)
    {
        builder.ToTable("user_location_assignments");

        builder.HasKey(ula => ula.Id);

        builder.Property(ula => ula.Id)
            .HasColumnName("id")
            .ValueGeneratedNever(); // We generate GUIDs in domain

        // User ID - required foreign key
        builder.Property(ula => ula.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.HasIndex(ula => ula.UserId)
            .HasDatabaseName("ix_user_location_assignments_user_id");

        // Location ID - required foreign key
        builder.Property(ula => ula.LocationId)
            .HasColumnName("location_id")
            .IsRequired();

        builder.HasIndex(ula => ula.LocationId)
            .HasDatabaseName("ix_user_location_assignments_location_id");

        // Composite index for user-location uniqueness (active assignments)
        builder.HasIndex(ula => new { ula.UserId, ula.LocationId })
            .HasDatabaseName("ix_user_location_assignments_user_location");

        // Location Role ID - optional foreign key
        builder.Property(ula => ula.LocationRoleId)
            .HasColumnName("location_role_id")
            .IsRequired(false);

        // Is Primary Location
        builder.Property(ula => ula.IsPrimaryLocation)
            .HasColumnName("is_primary_location")
            .IsRequired();

        // Assignment Status enum
        builder.Property(ula => ula.Status)
            .HasColumnName("status")
            .HasConversion<string>() // Store as string in database
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(ula => ula.Status)
            .HasDatabaseName("ix_user_location_assignments_status");

        // Assigned Date
        builder.Property(ula => ula.AssignedDate)
            .HasColumnName("assigned_date")
            .IsRequired();

        // Terminated Date
        builder.Property(ula => ula.TerminatedDate)
            .HasColumnName("terminated_date")
            .IsRequired(false);

        // Timestamps
        builder.Property(ula => ula.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(ula => ula.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        // Ignore domain events (not persisted)
        builder.Ignore(ula => ula.DomainEvents);
    }
}

