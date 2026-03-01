using Bintainer.Modules.Inventory.Domain.StorageUnits;

namespace Bintainer.Modules.Inventory.Domain.UnitTests;

public class BinTests
{
    [Fact]
    public void Bin_CreatedViaStorageUnit_HasCorrectProperties()
    {
        var unit = StorageUnit.Create("Test", 2, 1, 3, Guid.NewGuid());

        var bins = unit.Bins.ToList();
        bins.Should().HaveCount(2);

        var firstBin = bins.First(b => b.Column == 0 && b.Row == 0);
        firstBin.Id.Should().NotBeEmpty();
        firstBin.StorageUnitId.Should().Be(unit.Id);
        firstBin.Compartments.Should().HaveCount(3);
    }
}
