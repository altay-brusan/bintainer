using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Inventory.Application.Compartments.ActivateCompartment;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Inventory.Presentation.Compartments;

internal sealed class ActivateCompartment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/compartments/{compartmentId:guid}/restore", async (Guid compartmentId, ISender sender) =>
        {
            var command = new ActivateCompartmentCommand(compartmentId);

            var result = await sender.Send(command);

            return result.Match(
                () => Results.NoContent(),
                ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Compartments);
    }
}
