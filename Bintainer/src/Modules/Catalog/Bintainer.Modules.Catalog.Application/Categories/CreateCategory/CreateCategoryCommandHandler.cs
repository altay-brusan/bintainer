using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Catalog.Application.Abstractions.Data;
using Bintainer.Modules.Catalog.Domain.Categories;

namespace Bintainer.Modules.Catalog.Application.Categories.CreateCategory;

internal sealed class CreateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateCategoryCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (request.ParentId.HasValue)
        {
            var parent = await categoryRepository.GetByIdAsync(request.ParentId.Value, cancellationToken);
            if (parent is null)
            {
                return Result.Failure<Guid>(CategoryErrors.NotFound(request.ParentId.Value));
            }
        }

        var category = Category.Create(request.Name, request.ParentId);

        categoryRepository.Insert(category);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}
