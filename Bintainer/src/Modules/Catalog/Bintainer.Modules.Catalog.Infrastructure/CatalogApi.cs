using Bintainer.Modules.Catalog.IntegrationEvents;
using Bintainer.Modules.Catalog.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Bintainer.Modules.Catalog.Infrastructure;

internal sealed class CatalogApi(CatalogDbContext dbContext) : ICatalogApi
{
    public async Task<bool> ComponentExistsAsync(Guid componentId, CancellationToken ct = default)
    {
        return await dbContext.Components.AnyAsync(c => c.Id == componentId, ct);
    }
}
