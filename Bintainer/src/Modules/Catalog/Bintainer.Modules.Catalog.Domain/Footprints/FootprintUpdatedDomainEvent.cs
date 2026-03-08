using Bintainer.Common.Domain;

namespace Bintainer.Modules.Catalog.Domain.Footprints;

public sealed class FootprintUpdatedDomainEvent(Guid footprintId, string name) : DomainEvent
{
    public Guid FootprintId { get; init; } = footprintId;
    public string Name { get; init; } = name;
}
