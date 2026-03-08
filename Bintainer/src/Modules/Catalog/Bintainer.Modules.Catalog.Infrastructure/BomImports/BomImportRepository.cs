using Bintainer.Modules.Catalog.Domain.BomImports;
using Bintainer.Modules.Catalog.Infrastructure.Database;

namespace Bintainer.Modules.Catalog.Infrastructure.BomImports;

internal sealed class BomImportRepository(CatalogDbContext dbContext) : IBomImportRepository
{
    public void Insert(BomImport bomImport)
    {
        dbContext.BomImports.Add(bomImport);
    }
}
