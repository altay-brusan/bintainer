using Bintainer.Modules.Inventory.Domain.Inventories;

namespace Bintainer.Modules.Inventory.Domain.UnitTests;

public class InventoryTests
{
    [Fact]
    public void Create_ReturnsInventoryWithProperties()
    {
        var userId = Guid.NewGuid();

        var inventory = Domain.Inventories.Inventory.Create("My Inventory", userId);

        inventory.Name.Should().Be("My Inventory");
        inventory.UserId.Should().Be(userId);
    }

    [Fact]
    public void Create_GeneratesNewId()
    {
        var inventory = Domain.Inventories.Inventory.Create("Test", Guid.NewGuid());

        inventory.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_RaisesInventoryCreatedDomainEvent()
    {
        var inventory = Domain.Inventories.Inventory.Create("Test", Guid.NewGuid());

        inventory.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<InventoryCreatedDomainEvent>()
            .Which.InventoryId.Should().Be(inventory.Id);
    }

    [Fact]
    public void Create_StorageUnitsIsEmpty()
    {
        var inventory = Domain.Inventories.Inventory.Create("Test", Guid.NewGuid());

        inventory.StorageUnits.Should().BeEmpty();
    }
}
