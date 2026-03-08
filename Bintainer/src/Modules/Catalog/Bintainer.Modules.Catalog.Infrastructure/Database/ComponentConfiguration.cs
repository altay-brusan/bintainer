using Bintainer.Modules.Catalog.Domain.Categories;
using Bintainer.Modules.Catalog.Domain.Components;
using Bintainer.Modules.Catalog.Domain.Footprints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bintainer.Modules.Catalog.Infrastructure.Database;

internal sealed class ComponentConfiguration : IEntityTypeConfiguration<Component>
{
    public void Configure(EntityTypeBuilder<Component> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.PartNumber).HasMaxLength(100);
        builder.Property(p => p.ManufacturerPartNumber).HasMaxLength(200);
        builder.Property(p => p.Description).HasMaxLength(500);
        builder.Property(p => p.DetailedDescription).HasMaxLength(2000);
        builder.Property(p => p.ImageUrl).HasMaxLength(500);
        builder.Property(p => p.Url).HasMaxLength(500);
        builder.Property(p => p.Provider).HasMaxLength(50);
        builder.Property(p => p.ProviderPartNumber).HasMaxLength(200);

        builder.Property(p => p.Attributes)
            .HasColumnType("jsonb");

        builder.Property(p => p.Tags).HasMaxLength(500);
        builder.Property(p => p.UnitPrice).HasPrecision(18, 4);
        builder.Property(p => p.Manufacturer).HasMaxLength(200);
        builder.Property(p => p.LowStockThreshold).HasDefaultValue(0);

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<Footprint>()
            .WithMany()
            .HasForeignKey(p => p.FootprintId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
