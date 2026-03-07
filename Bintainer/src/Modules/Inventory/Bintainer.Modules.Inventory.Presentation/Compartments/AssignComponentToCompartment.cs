using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Inventory.Application.Compartments.AssignComponent;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Inventory.Presentation.Compartments;

internal sealed class AssignComponentToCompartment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("api/compartments/{compartmentId:guid}/component", async (Guid compartmentId, AssignComponentRequest request, ISender sender) =>
        {
            var command = new AssignComponentToCompartmentCommand(compartmentId, request.ComponentId, request.Quantity);

            var result = await sender.Send(command);

            return result.Match(
                () => Microsoft.AspNetCore.Http.Results.NoContent(),
                ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Compartments);
    }

    internal sealed record AssignComponentRequest(Guid ComponentId, int Quantity);
}
