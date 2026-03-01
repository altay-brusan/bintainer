using Bintainer.Common.Domain;
using Bintainer.Modules.Inventory.Domain.Inventories;

namespace Bintainer.Modules.Inventory.Domain.UnitTests;

public class InventoryErrorsTests
{
    [Fact]
    public void NotFound_ReturnsNotFoundError()
    {
        var inventoryId = Guid.NewGuid();

        var error = InventoryErrors.NotFound(inventoryId);

        error.Type.Should().Be(ErrorType.NotFound);
        error.Code.Should().Be("Inventories.NotFound");
        error.Description.Should().Contain(inventoryId.ToString());
    }
}
