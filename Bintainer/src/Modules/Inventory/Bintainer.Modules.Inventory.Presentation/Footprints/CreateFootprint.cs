using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Inventory.Application.Footprints.CreateFootprint;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Inventory.Presentation.Footprints;

internal sealed class CreateFootprint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/footprints", async (CreateFootprintRequest request, ISender sender) =>
        {
            var command = new CreateFootprintCommand(request.Name);

            var result = await sender.Send(command);

            return result.Match(
                id => Microsoft.AspNetCore.Http.Results.Created($"/api/footprints/{id}", id),
                ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Footprints);
    }

    internal sealed record CreateFootprintRequest(string Name);
}
