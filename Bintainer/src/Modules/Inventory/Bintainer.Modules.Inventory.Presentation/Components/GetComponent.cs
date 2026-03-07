using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Inventory.Application.Components.GetComponent;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Inventory.Presentation.Components;

internal sealed class GetComponent : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/components/{componentId:guid}", async (Guid componentId, ISender sender) =>
        {
            var query = new GetComponentQuery(componentId);

            var result = await sender.Send(query);

            return result.Match(
                response => Microsoft.AspNetCore.Http.Results.Ok(response),
                ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Components);
    }
}
