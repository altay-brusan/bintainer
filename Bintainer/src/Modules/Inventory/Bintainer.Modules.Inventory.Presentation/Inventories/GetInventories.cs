using Bintainer.Common.Application.Authorization;
using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Inventory.Application.Inventories.GetInventories;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Inventory.Presentation.Inventories;

internal sealed class GetInventories : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/inventories", async (ICurrentUserService currentUser, ISender sender) =>
        {
            var query = new GetInventoriesQuery(currentUser.UserId);

            var result = await sender.Send(query);

            return result.Match(
                response => Results.Ok(response),
                ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Inventories);
    }
}
