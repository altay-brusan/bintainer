using Bintainer.Modules.Inventory.Domain.StorageUnits;
using Bintainer.Modules.Inventory.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Bintainer.Modules.Inventory.Infrastructure.StorageUnits;

internal sealed class StorageUnitRepository(InventoryDbContext dbContext) : IStorageUnitRepository
{
    public async Task<StorageUnit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.StorageUnits.FirstOrDefaultAsync(su => su.Id == id, cancellationToken);
    }

    public async Task<StorageUnit?> GetByIdWithBinsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.StorageUnits
            .Include(su => su.Bins)
            .ThenInclude(b => b.Compartments)
            .FirstOrDefaultAsync(su => su.Id == id, cancellationToken);
    }

    public void Insert(StorageUnit storageUnit)
    {
        dbContext.StorageUnits.Add(storageUnit);
    }

    public void Remove(StorageUnit storageUnit)
    {
        dbContext.StorageUnits.Remove(storageUnit);
    }
}
