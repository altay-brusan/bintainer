using Bintainer.Common.Domain;
using Bintainer.Modules.Inventory.Domain.Bins;

namespace Bintainer.Modules.Inventory.Domain.StorageUnits;

public sealed class StorageUnit : Entity
{
    private readonly List<Bin> _bins = [];

    private StorageUnit() { }

    public string Name { get; private set; } = string.Empty;
    public int Columns { get; private set; }
    public int Rows { get; private set; }
    public int CompartmentCount { get; private set; }
    public Guid InventoryId { get; private set; }
    public IReadOnlyCollection<Bin> Bins => _bins.AsReadOnly();

    public static StorageUnit Create(string name, int columns, int rows, int compartmentCount, Guid inventoryId)
    {
        var storageUnit = new StorageUnit
        {
            Id = Guid.NewGuid(),
            Name = name,
            Columns = columns,
            Rows = rows,
            CompartmentCount = compartmentCount,
            InventoryId = inventoryId
        };

        for (int col = 0; col < columns; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                var bin = Bin.Create(col, row, storageUnit.Id, compartmentCount);
                storageUnit._bins.Add(bin);
            }
        }

        storageUnit.Raise(new StorageUnitCreatedDomainEvent(storageUnit.Id, name));

        return storageUnit;
    }

    public void Update(string name)
    {
        Name = name;
        Raise(new StorageUnitUpdatedDomainEvent(Id, name));
    }
}
