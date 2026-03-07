using Bintainer.Modules.Inventory.Domain.Compartments;
using Bintainer.Modules.Inventory.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Bintainer.Modules.Inventory.Infrastructure.Compartments;

internal sealed class CompartmentRepository(InventoryDbContext dbContext) : ICompartmentRepository
{
    public async Task<Compartment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Compartments.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<List<Compartment>> GetByComponentIdAsync(Guid componentId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Compartments
            .Where(c => c.ComponentId == componentId)
            .ToListAsync(cancellationToken);
    }
}
