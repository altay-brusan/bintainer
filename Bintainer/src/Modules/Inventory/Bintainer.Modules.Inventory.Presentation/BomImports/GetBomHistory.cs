using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Inventory.Application.BomImports.GetBomHistory;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Inventory.Presentation.BomImports;

internal sealed class GetBomHistory : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/bom/history", async (int? page, int? pageSize, ISender sender) =>
        {
            var query = new GetBomHistoryQuery(page ?? 1, pageSize ?? 20);

            var result = await sender.Send(query);

            return result.Match(
                response => Microsoft.AspNetCore.Http.Results.Ok(response),
                ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.BomImports);
    }
}
