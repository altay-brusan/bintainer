using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Inventory.Application.StorageUnits.GetStorageUnits;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Inventory.Presentation.StorageUnits;

internal sealed class GetStorageUnits : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/inventories/{inventoryId:guid}/storage-units", async (Guid inventoryId, ISender sender) =>
        {
            var query = new GetStorageUnitsQuery(inventoryId);

            var result = await sender.Send(query);

            return result.Match(
                response => Results.Ok(response),
                ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.StorageUnits);
    }
}
