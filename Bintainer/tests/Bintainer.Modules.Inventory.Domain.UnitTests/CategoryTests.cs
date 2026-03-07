using Bintainer.Modules.Inventory.Domain.Categories;

namespace Bintainer.Modules.Inventory.Domain.UnitTests;

public class CategoryTests
{
    [Fact]
    public void Create_RootCategory_HasNullParentId()
    {
        var category = Category.Create("Electronics");

        category.Name.Should().Be("Electronics");
        category.ParentId.Should().BeNull();
        category.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_ChildCategory_HasParentId()
    {
        var parentId = Guid.NewGuid();

        var category = Category.Create("Resistors", parentId);

        category.Name.Should().Be("Resistors");
        category.ParentId.Should().Be(parentId);
    }

    [Fact]
    public void Create_RaisesCategoryCreatedDomainEvent()
    {
        var category = Category.Create("Electronics");

        category.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<CategoryCreatedDomainEvent>()
            .Which.CategoryId.Should().Be(category.Id);
    }

    [Fact]
    public void Create_ChildrenIsEmpty()
    {
        var category = Category.Create("Electronics");

        category.Children.Should().BeEmpty();
    }

    [Fact]
    public void Update_ChangesNameAndParentId()
    {
        var category = Category.Create("Old Name");
        var newParentId = Guid.NewGuid();

        category.Update("New Name", newParentId);

        category.Name.Should().Be("New Name");
        category.ParentId.Should().Be(newParentId);
    }

    [Fact]
    public void Update_CanSetParentIdToNull()
    {
        var category = Category.Create("Child", Guid.NewGuid());

        category.Update("Child", null);

        category.ParentId.Should().BeNull();
    }
}
