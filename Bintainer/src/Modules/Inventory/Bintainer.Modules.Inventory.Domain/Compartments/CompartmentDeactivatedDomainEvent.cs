using Bintainer.Common.Domain;

namespace Bintainer.Modules.Inventory.Domain.Compartments;

public sealed class CompartmentDeactivatedDomainEvent(Guid compartmentId) : DomainEvent
{
    public Guid CompartmentId { get; init; } = compartmentId;
}
