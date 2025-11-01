using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

/// <summary>
/// Entity Framework configuration for PasswordHistory entity.
/// </summary>
public class PasswordHistoryConfiguration : IEntityTypeConfiguration<PasswordHistory>
{
    public void Configure(EntityTypeBuilder<PasswordHistory> builder)
    {
        builder.ToTable("password_history");

        builder.HasKey(ph => ph.Id);

        builder.Property(ph => ph.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(ph => ph.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        // PasswordHash value object
        builder.OwnsOne(ph => ph.PasswordHash, passwordHash => passwordHash.Property(p => p.Value)
                .HasColumnName("password_hash")
                .HasMaxLength(500)
                .IsRequired());

        builder.Property(ph => ph.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        // Index for efficient querying by user
        builder.HasIndex(ph => ph.UserId)
            .HasDatabaseName("ix_password_history_user_id");

        // Index for efficient querying by user and creation date
        builder.HasIndex(ph => new { ph.UserId, ph.CreatedAt })
            .HasDatabaseName("ix_password_history_user_id_created_at");

        // Ignore domain events
        builder.Ignore(ph => ph.DomainEvents);
    }
}

