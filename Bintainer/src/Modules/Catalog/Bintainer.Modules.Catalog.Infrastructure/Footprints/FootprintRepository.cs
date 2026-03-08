using Bintainer.Modules.Catalog.Domain.Footprints;
using Bintainer.Modules.Catalog.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Bintainer.Modules.Catalog.Infrastructure.Footprints;

internal sealed class FootprintRepository(CatalogDbContext dbContext) : IFootprintRepository
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
