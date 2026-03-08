using Bintainer.Modules.Inventory.Domain.Compartments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bintainer.Modules.Inventory.Infrastructure.Database;

internal sealed class CompartmentConfiguration : IEntityTypeConfiguration<Compartment>
{
    public void Configure(EntityTypeBuilder<Compartment> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Index);
        builder.Property(c => c.Label).HasMaxLength(100);
        builder.Property(c => c.BinId);
        builder.Property(c => c.Quantity).HasDefaultValue(0);
        builder.Property(c => c.ComponentId);
    }
}
