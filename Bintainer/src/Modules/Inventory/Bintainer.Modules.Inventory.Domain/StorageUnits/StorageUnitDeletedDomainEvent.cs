using Bintainer.Common.Domain;

namespace Bintainer.Modules.Inventory.Domain.StorageUnits;

public sealed class StorageUnitDeletedDomainEvent(Guid storageUnitId) : DomainEvent
{
    public Guid StorageUnitId { get; init; } = storageUnitId;
}
