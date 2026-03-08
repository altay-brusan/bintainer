using Bintainer.Common.Domain;

namespace Bintainer.Modules.Catalog.Domain.Categories;

public sealed class CategoryDeletedDomainEvent(Guid categoryId, string name) : DomainEvent
{
    public Guid CategoryId { get; init; } = categoryId;
    public string Name { get; init; } = name;
}
