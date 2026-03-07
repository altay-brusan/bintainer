using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Inventory.Application.Components.AdjustComponentQuantity;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Inventory.Presentation.Components;

internal sealed class AdjustComponentQuantity : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/components/{componentId:guid}/quantity", async (Guid componentId, AdjustQuantityRequest request, ISender sender) =>
        {
            var command = new AdjustComponentQuantityCommand(componentId, request.CompartmentId, request.Action, request.Quantity, request.Notes);

            var result = await sender.Send(command);

            return result.Match(
                () => Microsoft.AspNetCore.Http.Results.NoContent(),
                ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Components);
    }

    internal sealed record AdjustQuantityRequest(Guid CompartmentId, string Action, int Quantity, string? Notes);
}
