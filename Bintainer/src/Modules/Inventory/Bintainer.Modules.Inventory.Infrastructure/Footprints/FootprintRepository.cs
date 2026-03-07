using Bintainer.Modules.Inventory.Domain.Footprints;
using Bintainer.Modules.Inventory.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Bintainer.Modules.Inventory.Infrastructure.Footprints;

internal sealed class FootprintRepository(InventoryDbContext dbContext) : IFootprintRepository
{
    public async Task<Footprint?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Footprints.FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
    }

    public void Insert(Footprint footprint)
    {
        dbContext.Footprints.Add(footprint);
    }

    public void Remove(Footprint footprint)
    {
        dbContext.Footprints.Remove(footprint);
    }
}
