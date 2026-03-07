using Bintainer.Common.Domain;

namespace Bintainer.Modules.Inventory.Domain.Categories;

public sealed class CategoryCreatedDomainEvent(Guid categoryId) : DomainEvent
{
    public Guid CategoryId { get; init; } = categoryId;
}
