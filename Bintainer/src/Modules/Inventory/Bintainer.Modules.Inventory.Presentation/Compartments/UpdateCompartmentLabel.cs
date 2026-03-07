using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Inventory.Application.Compartments.UpdateLabel;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Inventory.Presentation.Compartments;

internal sealed class UpdateCompartmentLabel : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("api/compartments/{compartmentId:guid}/label", async (Guid compartmentId, UpdateLabelRequest request, ISender sender) =>
        {
            var command = new UpdateCompartmentLabelCommand(compartmentId, request.Label);

            var result = await sender.Send(command);

            return result.Match(
                () => Microsoft.AspNetCore.Http.Results.NoContent(),
                ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Compartments);
    }

    internal sealed record UpdateLabelRequest(string Label);
}
