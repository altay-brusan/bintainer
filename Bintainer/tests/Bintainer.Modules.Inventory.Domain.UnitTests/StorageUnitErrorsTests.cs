using Bintainer.Common.Domain;
using Bintainer.Modules.Inventory.Domain.StorageUnits;

namespace Bintainer.Modules.Inventory.Domain.UnitTests;

public class StorageUnitErrorsTests
{
    [Fact]
    public void NotFound_ReturnsNotFoundError()
    {
        var storageUnitId = Guid.NewGuid();

        var error = StorageUnitErrors.NotFound(storageUnitId);

        error.Type.Should().Be(ErrorType.NotFound);
        error.Code.Should().Be("StorageUnits.NotFound");
        error.Description.Should().Contain(storageUnitId.ToString());
    }
}
