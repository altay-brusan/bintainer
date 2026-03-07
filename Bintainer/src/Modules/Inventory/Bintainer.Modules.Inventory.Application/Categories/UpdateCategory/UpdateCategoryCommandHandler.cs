using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Domain.Categories;

namespace Bintainer.Modules.Inventory.Application.Categories.UpdateCategory;

internal sealed class UpdateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateCategoryCommand>
{
    public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        Category? category = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category is null)
        {
            return Result.Failure(CategoryErrors.NotFound(request.CategoryId));
        }

        if (request.ParentId.HasValue)
        {
            if (request.ParentId.Value == request.CategoryId)
            {
                return Result.Failure(CategoryErrors.CircularReference(request.CategoryId));
            }

            var parentId = request.ParentId.Value;
            while (true)
            {
                var parent = await categoryRepository.GetByIdAsync(parentId, cancellationToken);
                if (parent is null)
                {
                    return Result.Failure(CategoryErrors.NotFound(request.ParentId.Value));
                }

                if (!parent.ParentId.HasValue)
                {
                    break;
                }

                if (parent.ParentId.Value == request.CategoryId)
                {
                    return Result.Failure(CategoryErrors.CircularReference(request.CategoryId));
                }

                parentId = parent.ParentId.Value;
            }
        }

        category.Update(request.Name, request.ParentId);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
