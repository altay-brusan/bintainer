using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Inventory.Application.StorageUnits.CreateStorageUnit;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Inventory.Presentation.StorageUnits;

internal sealed class CreateStorageUnit : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/storage-units", async (CreateStorageUnitRequest request, ISender sender) =>
        {
            var command = new CreateStorageUnitCommand(
                request.Name, request.Columns, request.Rows,
                request.CompartmentCount, request.InventoryId);

            var result = await sender.Send(command);

            return result.Match(
                id => Microsoft.AspNetCore.Http.Results.Created($"/api/storage-units/{id}", id),
                ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.StorageUnits);
    }

    internal sealed record CreateStorageUnitRequest(string Name, int Columns, int Rows, int CompartmentCount, Guid InventoryId);
}
