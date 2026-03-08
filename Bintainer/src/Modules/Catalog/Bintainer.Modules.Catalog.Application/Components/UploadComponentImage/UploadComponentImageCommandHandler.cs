using Bintainer.Common.Application.ActivityLog;
using Bintainer.Common.Application.Authorization;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Catalog.Application.Abstractions.Data;
using Bintainer.Modules.Catalog.Application.Abstractions.Storage;
using Bintainer.Modules.Catalog.Domain.Components;

namespace Bintainer.Modules.Catalog.Application.Components.UploadComponentImage;

internal sealed class UploadComponentImageCommandHandler(
    IComponentRepository componentRepository,
    IFileStorageService fileStorageService,
    IActivityLogger activityLogger,
    ICurrentUserService currentUserService,
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

        await activityLogger.LogAsync(
            currentUserService.UserId,
            "ComponentImageUploaded",
            "Component",
            request.ComponentId,
            ct: cancellationToken);

        return url;
    }
}
