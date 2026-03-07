using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Categories.CreateCategory;

public sealed record CreateCategoryCommand(string Name, Guid? ParentId) : ICommand<Guid>;
