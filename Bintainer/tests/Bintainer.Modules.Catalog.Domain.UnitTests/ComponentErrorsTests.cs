using Bintainer.Common.Domain;
using Bintainer.Modules.Catalog.Domain.Components;

namespace Bintainer.Modules.Catalog.Domain.UnitTests;

public class ComponentErrorsTests
{
    [Fact]
    public void NotFound_ReturnsNotFoundError()
    {
        var componentId = Guid.NewGuid();

        var error = ComponentErrors.NotFound(componentId);

        error.Code.Should().Be("Components.NotFound");
        error.Type.Should().Be(ErrorType.NotFound);
    }
}
