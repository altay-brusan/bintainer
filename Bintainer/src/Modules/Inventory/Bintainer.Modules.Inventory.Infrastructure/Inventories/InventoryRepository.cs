using Bintainer.Modules.Inventory.Domain.Inventories;
using Bintainer.Modules.Inventory.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Bintainer.Modules.Inventory.Infrastructure.Inventories;

internal sealed class InventoryRepository(InventoryDbContext dbContext) : IInventoryRepository
{
    public async Task<Domain.Inventories.Inventory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Inventories.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public void Insert(Domain.Inventories.Inventory inventory)
    {
        dbContext.Inventories.Add(inventory);
    }
}
