using Bintainer.Modules.Inventory.Domain.StorageUnits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bintainer.Modules.Inventory.Infrastructure.Database;

internal sealed class StorageUnitConfiguration : IEntityTypeConfiguration<StorageUnit>
{
    public void Configure(EntityTypeBuilder<StorageUnit> builder)
    {
        builder.HasKey(su => su.Id);

        builder.Property(su => su.Name).HasMaxLength(200);
        builder.Property(su => su.Columns);
        builder.Property(su => su.Rows);
        builder.Property(su => su.CompartmentCount);
        builder.Property(su => su.InventoryId);

        builder.HasMany(su => su.Bins)
            .WithOne()
            .HasForeignKey(b => b.StorageUnitId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
