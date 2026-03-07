using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Inventory.Application.Movements.GetMovements;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Inventory.Presentation.Movements;

internal sealed class GetMovements : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/movements", async (string? action, Guid? componentId, string? q, int? page, int? pageSize, ISender sender) =>
        {
            var query = new GetMovementsQuery(action, componentId, q, page ?? 1, pageSize ?? 20);

            var result = await sender.Send(query);

            return result.Match(
                response => Microsoft.AspNetCore.Http.Results.Ok(response),
                ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Movements);
    }
}
