using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Inventory.Application.Components.MoveComponent;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Inventory.Presentation.Components;

internal sealed class MoveComponent : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/components/{componentId:guid}/move", async (Guid componentId, MoveComponentRequest request, ISender sender) =>
        {
            var command = new MoveComponentCommand(componentId, request.SourceCompartmentId, request.DestinationCompartmentId, request.Quantity);

            var result = await sender.Send(command);

            return result.Match(
                () => Microsoft.AspNetCore.Http.Results.NoContent(),
                ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Components);
    }

    internal sealed record MoveComponentRequest(Guid SourceCompartmentId, Guid DestinationCompartmentId, int Quantity);
}
