using Bintainer.Modules.Inventory.Domain.Bins;
using Bintainer.Modules.Inventory.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Bintainer.Modules.Inventory.Infrastructure.Bins;

internal sealed class BinRepository(InventoryDbContext dbContext) : IBinRepository
{
    public async Task<Bin?> GetByIdWithCompartmentsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Bins
            .Include(b => b.Compartments)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }
}
