using Bintainer.Common.Domain;

namespace Bintainer.Modules.Inventory.Domain.Footprints;

public sealed class Footprint : Entity
{
    private Footprint() { }

    public string Name { get; private set; } = string.Empty;

    public static Footprint Create(string name)
    {
        return new Footprint
        {
            Id = Guid.NewGuid(),
            Name = name
        };
    }

    public void Update(string name)
    {
        Name = name;
    }
}
