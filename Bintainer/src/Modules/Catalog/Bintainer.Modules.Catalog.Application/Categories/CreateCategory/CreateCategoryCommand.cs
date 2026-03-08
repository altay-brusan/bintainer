using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Catalog.Application.Categories.CreateCategory;

public sealed record CreateCategoryCommand(string Name, Guid? ParentId) : ICommand<Guid>;
