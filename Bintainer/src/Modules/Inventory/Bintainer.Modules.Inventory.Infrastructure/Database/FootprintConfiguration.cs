using Bintainer.Modules.Inventory.Domain.Footprints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bintainer.Modules.Inventory.Infrastructure.Database;

internal sealed class FootprintConfiguration : IEntityTypeConfiguration<Footprint>
{
    public void Configure(EntityTypeBuilder<Footprint> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Name).HasMaxLength(100);
    }
}
