using Bintainer.Modules.Catalog.Application.Abstractions.Data;
using Bintainer.Modules.Catalog.Domain.BomImports;
using Bintainer.Modules.Catalog.Domain.Categories;
using Bintainer.Modules.Catalog.Domain.Components;
using Bintainer.Modules.Catalog.Domain.Footprints;
using Microsoft.EntityFrameworkCore;

namespace Bintainer.Modules.Catalog.Infrastructure.Database;

public sealed class CatalogDbContext(DbContextOptions<CatalogDbContext> options)
    : DbContext(options), IUnitOfWork
{
    public DbSet<Component> Components => Set<Component>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Footprint> Footprints => Set<Footprint>();
    public DbSet<BomImport> BomImports => Set<BomImport>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Catalog);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
    }
}
