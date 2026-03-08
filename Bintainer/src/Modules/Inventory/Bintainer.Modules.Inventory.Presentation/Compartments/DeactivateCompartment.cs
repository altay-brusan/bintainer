using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Inventory.Application.Compartments.DeactivateCompartment;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Inventory.Presentation.Compartments;

internal sealed class DeactivateCompartment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/compartments/{compartmentId:guid}", async (Guid compartmentId, ISender sender) =>
        {
            var command = new DeactivateCompartmentCommand(compartmentId);

            var result = await sender.Send(command);

            return result.Match(
                () => Results.NoContent(),
                ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Compartments);
    }
}
