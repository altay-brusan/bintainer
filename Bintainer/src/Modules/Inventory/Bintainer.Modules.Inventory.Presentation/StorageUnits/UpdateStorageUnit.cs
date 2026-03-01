using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Inventory.Application.StorageUnits.UpdateStorageUnit;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Inventory.Presentation.StorageUnits;

internal sealed class UpdateStorageUnit : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("api/storage-units/{storageUnitId:guid}", async (Guid storageUnitId, UpdateStorageUnitRequest request, ISender sender) =>
        {
            var command = new UpdateStorageUnitCommand(storageUnitId, request.Name);

            var result = await sender.Send(command);

            return result.Match(
                () => Microsoft.AspNetCore.Http.Results.NoContent(),
                ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.StorageUnits);
    }

    internal sealed record UpdateStorageUnitRequest(string Name);
}
