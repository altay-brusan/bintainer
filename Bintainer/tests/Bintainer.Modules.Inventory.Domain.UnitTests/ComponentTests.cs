using Bintainer.Modules.Inventory.Domain.Components;

namespace Bintainer.Modules.Inventory.Domain.UnitTests;

public class ComponentTests
{
    [Fact]
    public void Create_ReturnsComponentWithProperties()
    {
        var categoryId = Guid.NewGuid();
        var footprintId = Guid.NewGuid();
        var attributes = new Dictionary<string, string> { ["Resistance"] = "10k" };

        var component = Component.Create(
            "PN-001", "MPN-001", "Resistor 10k",
            "Detailed desc", "https://img.com/1.png", "https://example.com",
            "DigiKey", "DK-001", categoryId, footprintId, attributes, "smd,passive",
            1.5m, "Yageo", 10);

        component.PartNumber.Should().Be("PN-001");
        component.ManufacturerPartNumber.Should().Be("MPN-001");
        component.Description.Should().Be("Resistor 10k");
        component.DetailedDescription.Should().Be("Detailed desc");
        component.ImageUrl.Should().Be("https://img.com/1.png");
        component.Url.Should().Be("https://example.com");
        component.Provider.Should().Be("DigiKey");
        component.ProviderPartNumber.Should().Be("DK-001");
        component.CategoryId.Should().Be(categoryId);
        component.FootprintId.Should().Be(footprintId);
        component.Attributes.Should().ContainKey("Resistance").WhoseValue.Should().Be("10k");
        component.Tags.Should().Be("smd,passive");
        component.UnitPrice.Should().Be(1.5m);
        component.Manufacturer.Should().Be("Yageo");
        component.LowStockThreshold.Should().Be(10);
    }

    [Fact]
    public void Create_GeneratesNewId()
    {
        var component = Component.Create("PN", "MPN", "Desc", null, null, null, null, null, null, null, null, null, null, null, 0);

        component.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_RaisesComponentCreatedDomainEvent()
    {
        var component = Component.Create("PN", "MPN", "Desc", null, null, null, null, null, null, null, null, null, null, null, 0);

        component.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ComponentCreatedDomainEvent>()
            .Which.ComponentId.Should().Be(component.Id);
    }

    [Fact]
    public void Create_WithNullAttributes_DefaultsToEmptyDictionary()
    {
        var component = Component.Create("PN", "MPN", "Desc", null, null, null, null, null, null, null, null, null, null, null, 0);

        component.Attributes.Should().BeEmpty();
    }

    [Fact]
    public void Update_ChangesAllProperties()
    {
        var component = Component.Create("PN", "MPN", "Desc", null, null, null, null, null, null, null, null, null, null, null, 0);
        var newCategoryId = Guid.NewGuid();
        var newFootprintId = Guid.NewGuid();

        component.Update("PN-2", "MPN-2", "Desc 2", "Detail", "https://img.com/2.png",
            "https://new.com", "Mouser", "MS-001", newCategoryId, newFootprintId,
            new Dictionary<string, string> { ["Voltage"] = "5V" }, "active,ic",
            2.5m, "TI", 5);

        component.PartNumber.Should().Be("PN-2");
        component.ManufacturerPartNumber.Should().Be("MPN-2");
        component.Description.Should().Be("Desc 2");
        component.DetailedDescription.Should().Be("Detail");
        component.ImageUrl.Should().Be("https://img.com/2.png");
        component.Url.Should().Be("https://new.com");
        component.Provider.Should().Be("Mouser");
        component.ProviderPartNumber.Should().Be("MS-001");
        component.CategoryId.Should().Be(newCategoryId);
        component.FootprintId.Should().Be(newFootprintId);
        component.Attributes.Should().ContainKey("Voltage");
        component.Tags.Should().Be("active,ic");
        component.UnitPrice.Should().Be(2.5m);
        component.Manufacturer.Should().Be("TI");
        component.LowStockThreshold.Should().Be(5);
    }

    [Fact]
    public void UpdateImageUrl_ChangesImageUrl()
    {
        var component = Component.Create("PN", "MPN", "Desc", null, null, null, null, null, null, null, null, null, null, null, 0);

        component.UpdateImageUrl("/uploads/test.png");

        component.ImageUrl.Should().Be("/uploads/test.png");
    }
}
