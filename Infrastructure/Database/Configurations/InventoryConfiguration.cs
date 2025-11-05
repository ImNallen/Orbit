using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

/// <summary>
/// Entity Framework configuration for Inventory entity.
/// </summary>
public class InventoryConfiguration : IEntityTypeConfiguration<Domain.Inventory.Inventory>
{
    public void Configure(EntityTypeBuilder<Domain.Inventory.Inventory> builder)
    {
        builder.ToTable("inventory");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasColumnName("id")
            .ValueGeneratedNever(); // We generate GUIDs in domain

        // Product reference
        builder.Property(i => i.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        builder.HasIndex(i => i.ProductId)
            .HasDatabaseName("ix_inventory_product_id");

        // Location reference
        builder.Property(i => i.LocationId)
            .HasColumnName("location_id")
            .IsRequired();

        builder.HasIndex(i => i.LocationId)
            .HasDatabaseName("ix_inventory_location_id");

        // Unique constraint: one inventory record per product-location combination
        builder.HasIndex(i => new { i.ProductId, i.LocationId })
            .IsUnique()
            .HasDatabaseName("ix_inventory_product_location");

        // Quantities
        builder.Property(i => i.Quantity)
            .HasColumnName("quantity")
            .IsRequired();

        builder.Property(i => i.ReservedQuantity)
            .HasColumnName("reserved_quantity")
            .IsRequired();

        // AvailableQuantity is a computed property in the domain model
        // We don't persist it to the database - it's calculated on the fly
        builder.Ignore(i => i.AvailableQuantity);

        // Timestamps
        builder.Property(i => i.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(i => i.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        // Ignore domain events (not persisted)
        builder.Ignore(i => i.DomainEvents);
    }
}

