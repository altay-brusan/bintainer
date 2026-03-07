using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Categories.UpdateCategory;

public sealed record UpdateCategoryCommand(Guid CategoryId, string Name, Guid? ParentId) : ICommand;
