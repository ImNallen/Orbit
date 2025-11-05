using Domain.Products;
using Domain.Products.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

/// <summary>
/// Entity Framework configuration for Product entity.
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id")
            .ValueGeneratedNever(); // We generate GUIDs in domain

        // Product name (Value Object)
        builder.Property(p => p.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .HasConversion(
                name => name.Value,
                value => ProductName.Create(value).Value)
            .IsRequired();

        // Product description (Value Object)
        builder.Property(p => p.Description)
            .HasColumnName("description")
            .HasMaxLength(2000)
            .HasConversion(
                description => description.Value,
                value => ProductDescription.Create(value).Value)
            .IsRequired();

        // Product price (Value Object - Money)
        // We'll store Amount and Currency separately
        builder.ComplexProperty(p => p.Price, priceBuilder =>
        {
            priceBuilder.Property(m => m.Amount)
                .HasColumnName("price")
                .HasPrecision(18, 2) // 18 digits total, 2 decimal places
                .IsRequired();

            priceBuilder.Property(m => m.Currency)
                .HasColumnName("currency")
                .HasMaxLength(3)
                .HasConversion(
                    currency => currency.Code,
                    code => Currency.Create(code).Value)
                .IsRequired();
        });

        // Product SKU (Value Object)
        builder.Property(p => p.Sku)
            .HasColumnName("sku")
            .HasMaxLength(50)
            .HasConversion(
                sku => sku.Value,
                value => Sku.Create(value).Value)
            .IsRequired();

        builder.HasIndex(p => p.Sku)
            .IsUnique()
            .HasDatabaseName("ix_products_sku");

        // Product status enum
        builder.Property(p => p.Status)
            .HasColumnName("status")
            .HasConversion<string>() // Store as string in database
            .HasMaxLength(20)
            .IsRequired();

        // Timestamps
        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        // Ignore domain events (not persisted)
        builder.Ignore(p => p.DomainEvents);
    }
}

