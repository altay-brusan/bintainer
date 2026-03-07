using Bintainer.Common.Domain;

namespace Bintainer.Modules.Inventory.Domain.Categories;

public sealed class Category : Entity
{
    private readonly List<Category> _children = [];

    private Category() { }

    public string Name { get; private set; } = string.Empty;
    public Guid? ParentId { get; private set; }
    public IReadOnlyCollection<Category> Children => _children.AsReadOnly();

    public static Category Create(string name, Guid? parentId = null)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = name,
            ParentId = parentId
        };

        category.Raise(new CategoryCreatedDomainEvent(category.Id));

        return category;
    }

    public void Update(string name, Guid? parentId)
    {
        Name = name;
        ParentId = parentId;
    }
}
