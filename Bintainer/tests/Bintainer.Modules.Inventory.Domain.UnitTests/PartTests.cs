using Bintainer.Modules.Inventory.Domain.Parts;

namespace Bintainer.Modules.Inventory.Domain.UnitTests;

public class PartTests
{
    [Fact]
    public void Create_ReturnsPartWithProperties()
    {
        var categoryId = Guid.NewGuid();
        var footprintId = Guid.NewGuid();
        var attributes = new Dictionary<string, string> { ["Resistance"] = "10k" };

        var part = Part.Create(
            "PN-001", "MPN-001", "Resistor 10k",
            "Detailed desc", "https://img.com/1.png", "https://example.com",
            "DigiKey", "DK-001", categoryId, footprintId, attributes, "smd,passive");

        part.PartNumber.Should().Be("PN-001");
        part.ManufacturerPartNumber.Should().Be("MPN-001");
        part.Description.Should().Be("Resistor 10k");
        part.DetailedDescription.Should().Be("Detailed desc");
        part.ImageUrl.Should().Be("https://img.com/1.png");
        part.Url.Should().Be("https://example.com");
        part.Provider.Should().Be("DigiKey");
        part.ProviderPartNumber.Should().Be("DK-001");
        part.CategoryId.Should().Be(categoryId);
        part.FootprintId.Should().Be(footprintId);
        part.Attributes.Should().ContainKey("Resistance").WhoseValue.Should().Be("10k");
        part.Tags.Should().Be("smd,passive");
    }

    [Fact]
    public void Create_GeneratesNewId()
    {
        var part = Part.Create("PN", "MPN", "Desc", null, null, null, null, null, null, null, null, null);

        part.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_RaisesPartCreatedDomainEvent()
    {
        var part = Part.Create("PN", "MPN", "Desc", null, null, null, null, null, null, null, null, null);

        part.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<PartCreatedDomainEvent>()
            .Which.PartId.Should().Be(part.Id);
    }

    [Fact]
    public void Create_WithNullAttributes_DefaultsToEmptyDictionary()
    {
        var part = Part.Create("PN", "MPN", "Desc", null, null, null, null, null, null, null, null, null);

        part.Attributes.Should().BeEmpty();
    }

    [Fact]
    public void Update_ChangesAllProperties()
    {
        var part = Part.Create("PN", "MPN", "Desc", null, null, null, null, null, null, null, null, null);
        var newCategoryId = Guid.NewGuid();
        var newFootprintId = Guid.NewGuid();

        part.Update("PN-2", "MPN-2", "Desc 2", "Detail", "https://img.com/2.png",
            "https://new.com", "Mouser", "MS-001", newCategoryId, newFootprintId,
            new Dictionary<string, string> { ["Voltage"] = "5V" }, "active,ic");

        part.PartNumber.Should().Be("PN-2");
        part.ManufacturerPartNumber.Should().Be("MPN-2");
        part.Description.Should().Be("Desc 2");
        part.DetailedDescription.Should().Be("Detail");
        part.ImageUrl.Should().Be("https://img.com/2.png");
        part.Url.Should().Be("https://new.com");
        part.Provider.Should().Be("Mouser");
        part.ProviderPartNumber.Should().Be("MS-001");
        part.CategoryId.Should().Be(newCategoryId);
        part.FootprintId.Should().Be(newFootprintId);
        part.Attributes.Should().ContainKey("Voltage");
        part.Tags.Should().Be("active,ic");
    }
}
