using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Inventory.Application.Bins.ActivateBin;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Inventory.Presentation.Bins;

internal sealed class ActivateBin : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/bins/{binId:guid}/restore", async (Guid binId, ISender sender) =>
        {
            var command = new ActivateBinCommand(binId);

            var result = await sender.Send(command);

            return result.Match(
                () => Results.NoContent(),
                ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Bins);
    }
}
