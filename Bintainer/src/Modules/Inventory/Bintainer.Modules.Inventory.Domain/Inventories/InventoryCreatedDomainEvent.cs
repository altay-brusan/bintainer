using Bintainer.Common.Domain;

namespace Bintainer.Modules.Inventory.Domain.Inventories;

public sealed class InventoryCreatedDomainEvent(Guid inventoryId) : DomainEvent
{
    public Guid InventoryId { get; init; } = inventoryId;
}
