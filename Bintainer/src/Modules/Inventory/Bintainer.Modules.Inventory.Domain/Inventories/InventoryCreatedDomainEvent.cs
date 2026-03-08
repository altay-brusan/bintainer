using Bintainer.Common.Domain;

namespace Bintainer.Modules.Inventory.Domain.Inventories;

public sealed class InventoryCreatedDomainEvent(Guid inventoryId, string name) : DomainEvent
{
    public Guid InventoryId { get; init; } = inventoryId;
    public string Name { get; init; } = name;
}
