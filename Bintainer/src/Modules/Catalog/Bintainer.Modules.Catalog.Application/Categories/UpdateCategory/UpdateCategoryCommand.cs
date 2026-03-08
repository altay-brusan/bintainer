using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Catalog.Application.Categories.UpdateCategory;

public sealed record UpdateCategoryCommand(Guid CategoryId, string Name, Guid? ParentId) : ICommand;
