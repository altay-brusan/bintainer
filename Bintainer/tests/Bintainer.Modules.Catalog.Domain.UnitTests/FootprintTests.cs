using Bintainer.Modules.Catalog.Domain.Footprints;

namespace Bintainer.Modules.Catalog.Domain.UnitTests;

public class FootprintTests
{
    [Fact]
    public void Create_ReturnsFootprintWithName()
    {
        var footprint = Footprint.Create("0402");

        footprint.Name.Should().Be("0402");
        footprint.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Update_ChangesName()
    {
        var footprint = Footprint.Create("0402");

        footprint.Update("0805");

        footprint.Name.Should().Be("0805");
    }
}
