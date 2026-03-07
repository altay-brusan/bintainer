using Bintainer.Modules.Inventory.Domain.StorageUnits;

namespace Bintainer.Modules.Inventory.Domain.UnitTests;

public class CompartmentTests
{
    [Fact]
    public void Compartment_CreatedViaStorageUnit_HasCorrectIndexAndLabel()
    {
        var unit = StorageUnit.Create("Test", 1, 1, 2, Guid.NewGuid());

        var compartments = unit.Bins.First().Compartments.OrderBy(c => c.Index).ToList();

        compartments[0].Index.Should().Be(0);
        compartments[0].Label.Should().Be("1-1-1");
        compartments[0].BinId.Should().NotBeEmpty();

        compartments[1].Index.Should().Be(1);
        compartments[1].Label.Should().Be("1-1-2");
    }

    [Fact]
    public void Compartment_InitialState_HasNoComponentAndZeroQuantity()
    {
        var unit = StorageUnit.Create("Test", 1, 1, 1, Guid.NewGuid());
        var compartment = unit.Bins.First().Compartments.First();

        compartment.ComponentId.Should().BeNull();
        compartment.Quantity.Should().Be(0);
    }

    [Fact]
    public void AssignComponent_SetsComponentIdAndQuantity()
    {
        var unit = StorageUnit.Create("Test", 1, 1, 1, Guid.NewGuid());
        var compartment = unit.Bins.First().Compartments.First();
        var componentId = Guid.NewGuid();

        compartment.AssignComponent(componentId, 42);

        compartment.ComponentId.Should().Be(componentId);
        compartment.Quantity.Should().Be(42);
    }

    [Fact]
    public void RemoveComponent_ClearsComponentIdAndQuantity()
    {
        var unit = StorageUnit.Create("Test", 1, 1, 1, Guid.NewGuid());
        var compartment = unit.Bins.First().Compartments.First();
        compartment.AssignComponent(Guid.NewGuid(), 10);

        compartment.RemoveComponent();

        compartment.ComponentId.Should().BeNull();
        compartment.Quantity.Should().Be(0);
    }

    [Fact]
    public void AdjustQuantity_IncreasesQuantity()
    {
        var unit = StorageUnit.Create("Test", 1, 1, 1, Guid.NewGuid());
        var compartment = unit.Bins.First().Compartments.First();
        compartment.AssignComponent(Guid.NewGuid(), 10);

        compartment.AdjustQuantity(5);

        compartment.Quantity.Should().Be(15);
    }

    [Fact]
    public void AdjustQuantity_DecreasesQuantity()
    {
        var unit = StorageUnit.Create("Test", 1, 1, 1, Guid.NewGuid());
        var compartment = unit.Bins.First().Compartments.First();
        compartment.AssignComponent(Guid.NewGuid(), 10);

        compartment.AdjustQuantity(-3);

        compartment.Quantity.Should().Be(7);
    }

    [Fact]
    public void AdjustQuantity_BelowZero_ClampsToZero()
    {
        var unit = StorageUnit.Create("Test", 1, 1, 1, Guid.NewGuid());
        var compartment = unit.Bins.First().Compartments.First();
        compartment.AssignComponent(Guid.NewGuid(), 5);

        compartment.AdjustQuantity(-10);

        compartment.Quantity.Should().Be(0);
    }

    [Fact]
    public void UpdateLabel_ChangesLabel()
    {
        var unit = StorageUnit.Create("Test", 1, 1, 1, Guid.NewGuid());
        var compartment = unit.Bins.First().Compartments.First();

        compartment.UpdateLabel("Custom Label");

        compartment.Label.Should().Be("Custom Label");
    }
}
