using Domain.Users.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

/// <summary>
/// Entity Framework configuration for Session entity.
/// </summary>
public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("sessions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(s => s.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(s => s.RefreshToken)
            .HasColumnName("refresh_token")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(s => s.IpAddress)
            .HasColumnName("ip_address")
            .HasMaxLength(45) // IPv6 max length
            .IsRequired();

        builder.Property(s => s.UserAgent)
            .HasColumnName("user_agent")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(s => s.ExpiresAt)
            .HasColumnName("expires_at")
            .IsRequired();

        builder.Property(s => s.LastAccessedAt)
            .HasColumnName("last_accessed_at")
            .IsRequired();

        builder.Property(s => s.IsRevoked)
            .HasColumnName("is_revoked")
            .IsRequired();

        builder.Property(s => s.RevokedAt)
            .HasColumnName("revoked_at");

        // Index on user_id for efficient querying of user sessions
        builder.HasIndex(s => s.UserId)
            .HasDatabaseName("ix_sessions_user_id");

        // Unique index on refresh_token for fast lookups
        builder.HasIndex(s => s.RefreshToken)
            .IsUnique()
            .HasDatabaseName("ix_sessions_refresh_token");

        // Index for finding expired sessions
        builder.HasIndex(s => s.ExpiresAt)
            .HasDatabaseName("ix_sessions_expires_at");

        // Composite index for finding active sessions
        builder.HasIndex(s => new { s.UserId, s.IsRevoked, s.ExpiresAt })
            .HasDatabaseName("ix_sessions_user_id_is_revoked_expires_at");

        // Ignore domain events
        builder.Ignore(s => s.DomainEvents);
    }
}

