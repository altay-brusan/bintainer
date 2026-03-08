using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Catalog.Application.Categories.DeleteCategory;

public sealed record DeleteCategoryCommand(Guid CategoryId) : ICommand;
