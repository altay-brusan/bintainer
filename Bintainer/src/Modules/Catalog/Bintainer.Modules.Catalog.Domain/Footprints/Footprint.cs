using Bintainer.Common.Domain;

namespace Bintainer.Modules.Catalog.Domain.Footprints;

public sealed class Footprint : Entity
{
    private Footprint() { }

    public string Name { get; private set; } = string.Empty;

    public static Footprint Create(string name)
    {
        var footprint = new Footprint
        {
            Id = Guid.NewGuid(),
            Name = name
        };

        footprint.Raise(new FootprintCreatedDomainEvent(footprint.Id, name));

        return footprint;
    }

    public void Update(string name)
    {
        Name = name;

        Raise(new FootprintUpdatedDomainEvent(Id, name));
    }
}
