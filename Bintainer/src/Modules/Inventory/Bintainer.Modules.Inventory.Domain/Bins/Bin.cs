using Bintainer.Common.Domain;
using Bintainer.Modules.Inventory.Domain.Compartments;

namespace Bintainer.Modules.Inventory.Domain.Bins;

public sealed class Bin : Entity
{
    private readonly List<Compartment> _compartments = [];

    private Bin() { }

    public int Column { get; private set; }
    public int Row { get; private set; }
    public Guid StorageUnitId { get; private set; }
    public IReadOnlyCollection<Compartment> Compartments => _compartments.AsReadOnly();

    internal static Bin Create(int column, int row, Guid storageUnitId, int compartmentCount)
    {
        var bin = new Bin
        {
            Id = Guid.NewGuid(),
            Column = column,
            Row = row,
            StorageUnitId = storageUnitId
        };

        for (int i = 0; i < compartmentCount; i++)
        {
            var compartment = Compartment.Create(i, $"{column + 1}-{row + 1}-{i + 1}", bin.Id);
            bin._compartments.Add(compartment);
        }

        return bin;
    }
}
