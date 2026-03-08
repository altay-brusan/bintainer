using Bintainer.Common.Domain;

namespace Bintainer.Modules.Catalog.Domain.Categories;

public sealed class CategoryCreatedDomainEvent(Guid categoryId) : DomainEvent
{
    public Guid CategoryId { get; init; } = categoryId;
}
