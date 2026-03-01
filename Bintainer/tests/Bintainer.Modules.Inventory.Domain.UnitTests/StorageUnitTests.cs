using Bintainer.Modules.Inventory.Domain.StorageUnits;

namespace Bintainer.Modules.Inventory.Domain.UnitTests;

public class StorageUnitTests
{
    [Fact]
    public void Create_ReturnsStorageUnitWithProperties()
    {
        var inventoryId = Guid.NewGuid();

        var unit = StorageUnit.Create("Shelf A", 3, 2, 4, inventoryId);

        unit.Name.Should().Be("Shelf A");
        unit.Columns.Should().Be(3);
        unit.Rows.Should().Be(2);
        unit.CompartmentCount.Should().Be(4);
        unit.InventoryId.Should().Be(inventoryId);
    }

    [Fact]
    public void Create_GeneratesNewId()
    {
        var unit = StorageUnit.Create("Test", 1, 1, 1, Guid.NewGuid());

        unit.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_RaisesDomainEvent()
    {
        var unit = StorageUnit.Create("Test", 1, 1, 1, Guid.NewGuid());

        unit.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<StorageUnitCreatedDomainEvent>()
            .Which.StorageUnitId.Should().Be(unit.Id);
    }

    [Fact]
    public void Create_GeneratesBinGrid_ColumnsTimesRows()
    {
        var unit = StorageUnit.Create("Test", 3, 2, 1, Guid.NewGuid());

        unit.Bins.Should().HaveCount(6); // 3 columns * 2 rows
    }

    [Fact]
    public void Create_EachBinHasCorrectCompartmentCount()
    {
        var unit = StorageUnit.Create("Test", 2, 2, 3, Guid.NewGuid());

        foreach (var bin in unit.Bins)
        {
            bin.Compartments.Should().HaveCount(3);
        }
    }

    [Fact]
    public void Create_BinsHaveCorrectColumnAndRowValues()
    {
        var unit = StorageUnit.Create("Test", 2, 3, 1, Guid.NewGuid());

        var bins = unit.Bins.ToList();
        bins.Should().HaveCount(6);

        // Verify all (col, row) pairs exist
        for (int col = 0; col < 2; col++)
        {
            for (int row = 0; row < 3; row++)
            {
                bins.Should().Contain(b => b.Column == col && b.Row == row);
            }
        }
    }

    [Fact]
    public void Create_CompartmentsHaveCorrectLabels()
    {
        var unit = StorageUnit.Create("Test", 1, 1, 2, Guid.NewGuid());

        var compartments = unit.Bins.First().Compartments.ToList();
        compartments.Should().HaveCount(2);
        compartments[0].Label.Should().Be("1-1-1");
        compartments[1].Label.Should().Be("1-1-2");
    }

    [Fact]
    public void Update_ChangesName()
    {
        var unit = StorageUnit.Create("Old Name", 1, 1, 1, Guid.NewGuid());

        unit.Update("New Name");

        unit.Name.Should().Be("New Name");
    }
}
