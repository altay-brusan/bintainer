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
}
