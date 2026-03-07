using Bintainer.Modules.Inventory.Domain.Components;
using Bintainer.Modules.Inventory.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Bintainer.Modules.Inventory.Infrastructure.Components;

internal sealed class ComponentRepository(InventoryDbContext dbContext) : IComponentRepository
{
    public async Task<Component?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Components.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Component?> GetByPartNumberAsync(string partNumber, CancellationToken cancellationToken = default)
    {
        return await dbContext.Components.FirstOrDefaultAsync(p => p.PartNumber == partNumber, cancellationToken);
    }

    public void Insert(Component component)
    {
        dbContext.Components.Add(component);
    }

    public void Remove(Component component)
    {
        dbContext.Components.Remove(component);
    }
}
