using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Categories.DeleteCategory;

public sealed record DeleteCategoryCommand(Guid CategoryId) : ICommand;
