using Bintainer.Common.Domain;

namespace Bintainer.Modules.Inventory.Domain.Compartments;

public sealed class Compartment : Entity
{
    private Compartment() { }

    public int Index { get; private set; }
    public string Label { get; private set; } = string.Empty;
    public Guid BinId { get; private set; }
    public Guid? ComponentId { get; private set; }
    public int Quantity { get; private set; }
    public bool IsActive { get; private set; } = true;

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

    public void AssignComponent(Guid componentId, int quantity)
    {
        ComponentId = componentId;
        Quantity = quantity;
        Raise(new CompartmentComponentAssignedDomainEvent(Id, componentId, quantity));
    }

    public void RemoveComponent()
    {
        ComponentId = null;
        Quantity = 0;
        Raise(new CompartmentComponentRemovedDomainEvent(Id));
    }

    public void AdjustQuantity(int delta)
    {
        var newQuantity = Quantity + delta;
        if (newQuantity < 0)
        {
            newQuantity = 0;
        }
        Quantity = newQuantity;
    }

    public void UpdateLabel(string label)
    {
        Label = label;
        Raise(new CompartmentLabelUpdatedDomainEvent(Id, label));
    }

    public Result Deactivate()
    {
        if (ComponentId.HasValue)
        {
            return Result.Failure(CompartmentErrors.HasComponent(Id));
        }

        Raise(new CompartmentDeactivatedDomainEvent(Id));
        IsActive = false;
        return Result.Success();
    }

    public void Activate()
    {
        IsActive = true;
        Raise(new CompartmentActivatedDomainEvent(Id));
    }
}
