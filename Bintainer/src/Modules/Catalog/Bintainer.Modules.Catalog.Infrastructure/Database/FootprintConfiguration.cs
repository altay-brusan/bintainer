using Bintainer.Modules.Catalog.Domain.Footprints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bintainer.Modules.Catalog.Infrastructure.Database;

internal sealed class FootprintConfiguration : IEntityTypeConfiguration<Footprint>
{
    public void Configure(EntityTypeBuilder<Footprint> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Name).HasMaxLength(100);
    }
}
