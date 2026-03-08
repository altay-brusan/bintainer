using Bintainer.Common.Domain;

namespace Bintainer.Modules.Inventory.Domain.StorageUnits;

public sealed class StorageUnitCreatedDomainEvent(Guid storageUnitId, string name) : DomainEvent
{
    public Guid StorageUnitId { get; init; } = storageUnitId;
    public string Name { get; init; } = name;
}
