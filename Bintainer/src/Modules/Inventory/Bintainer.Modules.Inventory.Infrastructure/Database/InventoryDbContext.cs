using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Domain.Bins;
using Bintainer.Modules.Inventory.Domain.BomImports;
using Bintainer.Modules.Inventory.Domain.Categories;
using Bintainer.Modules.Inventory.Domain.Compartments;
using Bintainer.Modules.Inventory.Domain.Components;
using Bintainer.Modules.Inventory.Domain.Footprints;
using Bintainer.Modules.Inventory.Domain.Movements;
using Bintainer.Modules.Inventory.Domain.StorageUnits;
using Microsoft.EntityFrameworkCore;

namespace Bintainer.Modules.Inventory.Infrastructure.Database;

public sealed class InventoryDbContext(DbContextOptions<InventoryDbContext> options)
    : DbContext(options), IUnitOfWork
{
    public DbSet<Domain.Inventories.Inventory> Inventories => Set<Domain.Inventories.Inventory>();
    public DbSet<StorageUnit> StorageUnits => Set<StorageUnit>();
    public DbSet<Bin> Bins => Set<Bin>();
    public DbSet<Compartment> Compartments => Set<Compartment>();
    public DbSet<Component> Components => Set<Component>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Footprint> Footprints => Set<Footprint>();
    public DbSet<Movement> Movements => Set<Movement>();
    public DbSet<BomImport> BomImports => Set<BomImport>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Inventory);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventoryDbContext).Assembly);
    }
}
