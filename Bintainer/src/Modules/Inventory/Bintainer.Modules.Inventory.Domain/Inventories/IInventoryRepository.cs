namespace Bintainer.Modules.Inventory.Domain.Inventories;

public interface IInventoryRepository
{
    Task<Inventory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Insert(Inventory inventory);
}
