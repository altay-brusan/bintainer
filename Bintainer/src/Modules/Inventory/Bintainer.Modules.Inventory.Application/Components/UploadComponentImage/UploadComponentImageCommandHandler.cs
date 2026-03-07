using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Application.Abstractions.Storage;
using Bintainer.Modules.Inventory.Domain.Components;

namespace Bintainer.Modules.Inventory.Application.Components.UploadComponentImage;

internal sealed class UploadComponentImageCommandHandler(
    IComponentRepository componentRepository,
    IFileStorageService fileStorageService,
    IUnitOfWork unitOfWork) : ICommandHandler<UploadComponentImageCommand, string>
{
    public async Task<Result<string>> Handle(UploadComponentImageCommand request, CancellationToken cancellationToken)
    {
        Component? component = await componentRepository.GetByIdAsync(request.ComponentId, cancellationToken);

        if (component is null)
        {
            return Result.Failure<string>(ComponentErrors.NotFound(request.ComponentId));
        }

        var url = await fileStorageService.SaveFileAsync(
            request.FileStream, request.FileName, request.ContentType, cancellationToken);

        component.UpdateImageUrl(url);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return url;
    }
}
