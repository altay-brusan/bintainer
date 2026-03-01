namespace Bintainer.Modules.Inventory.Domain.StorageUnits;

public interface IStorageUnitRepository
{
    Task<StorageUnit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<StorageUnit?> GetByIdWithBinsAsync(Guid id, CancellationToken cancellationToken = default);
    void Insert(StorageUnit storageUnit);
    void Remove(StorageUnit storageUnit);
}
