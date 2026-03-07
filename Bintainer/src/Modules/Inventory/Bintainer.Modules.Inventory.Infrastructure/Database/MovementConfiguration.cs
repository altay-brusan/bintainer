using Bintainer.Modules.Inventory.Domain.Components;
using Bintainer.Modules.Inventory.Domain.Compartments;
using Bintainer.Modules.Inventory.Domain.Movements;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bintainer.Modules.Inventory.Infrastructure.Database;

internal sealed class MovementConfiguration : IEntityTypeConfiguration<Movement>
{
    public void Configure(EntityTypeBuilder<Movement> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Date);
        builder.Property(m => m.Action).HasMaxLength(20);
        builder.Property(m => m.Quantity);
        builder.Property(m => m.UserId);
        builder.Property(m => m.Notes).HasMaxLength(500);

        builder.HasOne<Component>()
            .WithMany()
            .HasForeignKey(m => m.ComponentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Compartment>()
            .WithMany()
            .HasForeignKey(m => m.CompartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(m => m.Date);
        builder.HasIndex(m => m.ComponentId);
    }
}
