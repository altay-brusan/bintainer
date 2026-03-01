using Bintainer.Modules.Inventory.Domain.Bins;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bintainer.Modules.Inventory.Infrastructure.Database;

internal sealed class BinConfiguration : IEntityTypeConfiguration<Bin>
{
    public void Configure(EntityTypeBuilder<Bin> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Column);
        builder.Property(b => b.Row);
        builder.Property(b => b.StorageUnitId);

        builder.HasMany(b => b.Compartments)
            .WithOne()
            .HasForeignKey(c => c.BinId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
