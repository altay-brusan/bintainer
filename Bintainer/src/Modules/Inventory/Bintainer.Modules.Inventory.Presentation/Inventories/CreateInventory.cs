using Bintainer.Common.Application.Authorization;
using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Inventory.Application.Inventories.CreateInventory;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Inventory.Presentation.Inventories;

internal sealed class CreateInventory : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/inventories", async (CreateInventoryRequest request, ICurrentUserService currentUser, ISender sender) =>
        {
            var command = new CreateInventoryCommand(request.Name, currentUser.UserId);

            var result = await sender.Send(command);

            return result.Match(
                id => Microsoft.AspNetCore.Http.Results.Created($"/api/inventories/{id}", id),
                ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Inventories);
    }

    internal sealed record CreateInventoryRequest(string Name);
}
