using Bintainer.Modules.Inventory.Domain.Movements;
using Bintainer.Modules.Inventory.Infrastructure.Database;

namespace Bintainer.Modules.Inventory.Infrastructure.Movements;

internal sealed class MovementRepository(InventoryDbContext dbContext) : IMovementRepository
{
    public void Insert(Movement movement)
    {
        dbContext.Movements.Add(movement);
    }
}
