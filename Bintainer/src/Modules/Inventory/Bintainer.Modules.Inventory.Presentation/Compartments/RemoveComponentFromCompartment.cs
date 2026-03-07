using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Inventory.Application.Compartments.RemoveComponent;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Inventory.Presentation.Compartments;

internal sealed class RemoveComponentFromCompartment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/compartments/{compartmentId:guid}/component", async (Guid compartmentId, ISender sender) =>
        {
            var command = new RemoveComponentFromCompartmentCommand(compartmentId);

            var result = await sender.Send(command);

            return result.Match(
                () => Microsoft.AspNetCore.Http.Results.NoContent(),
                ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Compartments);
    }
}
