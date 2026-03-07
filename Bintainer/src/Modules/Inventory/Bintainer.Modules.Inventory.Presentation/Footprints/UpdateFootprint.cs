using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Inventory.Application.Footprints.UpdateFootprint;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Inventory.Presentation.Footprints;

internal sealed class UpdateFootprint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("api/footprints/{footprintId:guid}", async (Guid footprintId, UpdateFootprintRequest request, ISender sender) =>
        {
            var command = new UpdateFootprintCommand(footprintId, request.Name);

            var result = await sender.Send(command);

            return result.Match(
                () => Microsoft.AspNetCore.Http.Results.NoContent(),
                ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Footprints);
    }

    internal sealed record UpdateFootprintRequest(string Name);
}
