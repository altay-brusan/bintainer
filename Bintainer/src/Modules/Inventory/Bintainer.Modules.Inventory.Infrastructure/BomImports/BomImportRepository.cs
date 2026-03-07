using Bintainer.Modules.Inventory.Domain.BomImports;
using Bintainer.Modules.Inventory.Infrastructure.Database;

namespace Bintainer.Modules.Inventory.Infrastructure.BomImports;

internal sealed class BomImportRepository(InventoryDbContext dbContext) : IBomImportRepository
{
    public void Insert(BomImport bomImport)
    {
        dbContext.BomImports.Add(bomImport);
    }
}
