using Bintainer.Modules.Inventory.Domain.BomImports;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bintainer.Modules.Inventory.Infrastructure.Database;

internal sealed class BomImportConfiguration : IEntityTypeConfiguration<BomImport>
{
    public void Configure(EntityTypeBuilder<BomImport> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.FileName).HasMaxLength(500);
        builder.Property(b => b.TotalLines);
        builder.Property(b => b.MatchedCount);
        builder.Property(b => b.NewCount);
        builder.Property(b => b.TotalValue).HasPrecision(18, 4);
        builder.Property(b => b.UserId);
        builder.Property(b => b.Date);
    }
}
