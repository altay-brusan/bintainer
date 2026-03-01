using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bintainer.Modules.Inventory.Infrastructure.Database;

internal sealed class InventoryConfiguration : IEntityTypeConfiguration<Domain.Inventories.Inventory>
{
    public void Configure(EntityTypeBuilder<Domain.Inventories.Inventory> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Name).HasMaxLength(200);
        builder.Property(i => i.UserId);

        builder.HasMany(i => i.StorageUnits)
            .WithOne()
            .HasForeignKey(su => su.InventoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
