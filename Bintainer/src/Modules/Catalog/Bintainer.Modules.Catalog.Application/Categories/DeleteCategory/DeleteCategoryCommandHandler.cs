using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Catalog.Application.Abstractions.Data;
using Bintainer.Modules.Catalog.Domain.Categories;

namespace Bintainer.Modules.Catalog.Application.Categories.DeleteCategory;

internal sealed class DeleteCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteCategoryCommand>
{
    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        Category? category = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category is null)
        {
            return Result.Failure(CategoryErrors.NotFound(request.CategoryId));
        }

        bool hasChildren = await categoryRepository.HasChildrenAsync(request.CategoryId, cancellationToken);

        if (hasChildren)
        {
            return Result.Failure(CategoryErrors.HasChildren(request.CategoryId));
        }

        categoryRepository.Remove(category);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
