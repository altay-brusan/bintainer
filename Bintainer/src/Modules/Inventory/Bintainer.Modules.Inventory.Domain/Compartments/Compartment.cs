using Bintainer.Common.Domain;

namespace Bintainer.Modules.Inventory.Domain.Compartments;

public sealed class Compartment : Entity
{
    private Compartment() { }

    public int Index { get; private set; }
    public string Label { get; private set; } = string.Empty;
    public Guid BinId { get; private set; }

    internal static Compartment Create(int index, string label, Guid binId)
    {
        return new Compartment
        {
            Id = Guid.NewGuid(),
            Index = index,
            Label = label,
            BinId = binId
        };
    }
}
