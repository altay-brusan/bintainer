using Bintainer.Common.Domain;

namespace Bintainer.Modules.Inventory.Domain.Bins;

public sealed class BinActivatedDomainEvent(Guid binId) : DomainEvent
{
    public Guid BinId { get; init; } = binId;
}
