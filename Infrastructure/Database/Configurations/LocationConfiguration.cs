using Domain.Locations;
using Domain.Locations.ValueObjects;
using Domain.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

/// <summary>
/// Entity Framework configuration for Location entity.
/// </summary>
public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .HasColumnName("id")
            .ValueGeneratedNever(); // We generate GUIDs in domain

        // Location name (Value Object)
        builder.Property(l => l.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .HasConversion(
                name => name.Value,
                value => LocationName.Create(value).Value)
            .IsRequired();

        builder.HasIndex(l => l.Name)
            .HasDatabaseName("ix_locations_name");

        // Location type enum
        builder.Property(l => l.Type)
            .HasColumnName("type")
            .HasConversion<string>() // Store as string in database
            .HasMaxLength(50)
            .IsRequired();

        // Address value object
        builder.OwnsOne(l => l.Address, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("street")
                .HasMaxLength(200)
                .IsRequired();

            address.Property(a => a.City)
                .HasColumnName("city")
                .HasMaxLength(100)
                .IsRequired();

            address.Property(a => a.State)
                .HasColumnName("state")
                .HasMaxLength(100)
                .IsRequired(false); // Optional - not commonly used in Nordic countries

            address.Property(a => a.Country)
                .HasColumnName("country")
                .HasMaxLength(100)
                .IsRequired();

            address.Property(a => a.ZipCode)
                .HasColumnName("zip_code")
                .HasMaxLength(20)
                .IsRequired();
        });

        // Location status enum
        builder.Property(l => l.Status)
            .HasColumnName("status")
            .HasConversion<string>() // Store as string in database
            .HasMaxLength(50)
            .IsRequired();

        // Timestamps
        builder.Property(l => l.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(l => l.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        // Ignore domain events (not persisted)
        builder.Ignore(l => l.DomainEvents);
    }
}

