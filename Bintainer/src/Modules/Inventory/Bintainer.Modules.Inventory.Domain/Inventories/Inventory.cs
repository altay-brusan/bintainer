using Bintainer.Common.Domain;
using Bintainer.Modules.Inventory.Domain.StorageUnits;

namespace Bintainer.Modules.Inventory.Domain.Inventories;

public sealed class Inventory : Entity
{
    private readonly List<StorageUnit> _storageUnits = [];

    private Inventory() { }

    public string Name { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }
    public IReadOnlyCollection<StorageUnit> StorageUnits => _storageUnits.AsReadOnly();

    public static Inventory Create(string name, Guid userId)
    {
        var inventory = new Inventory
        {
            Id = Guid.NewGuid(),
            Name = name,
            UserId = userId
        };

        inventory.Raise(new InventoryCreatedDomainEvent(inventory.Id));

        return inventory;
    }
}
