using Bintainer.Common.Domain;
using Bintainer.Modules.Inventory.Domain.Categories;

namespace Bintainer.Modules.Inventory.Domain.UnitTests;

public class CategoryErrorsTests
{
    [Fact]
    public void NotFound_ReturnsNotFoundError()
    {
        var categoryId = Guid.NewGuid();

        var error = CategoryErrors.NotFound(categoryId);

        error.Code.Should().Be("Categories.NotFound");
        error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public void HasChildren_ReturnsConflictError()
    {
        var categoryId = Guid.NewGuid();

        var error = CategoryErrors.HasChildren(categoryId);

        error.Code.Should().Be("Categories.HasChildren");
        error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public void CircularReference_ReturnsConflictError()
    {
        var categoryId = Guid.NewGuid();

        var error = CategoryErrors.CircularReference(categoryId);

        error.Code.Should().Be("Categories.CircularReference");
        error.Type.Should().Be(ErrorType.Conflict);
    }
}
