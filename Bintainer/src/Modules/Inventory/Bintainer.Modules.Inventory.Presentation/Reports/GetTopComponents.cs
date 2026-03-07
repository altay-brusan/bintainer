using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Inventory.Application.Reports.GetTopComponents;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Inventory.Presentation.Reports;

internal sealed class GetTopComponents : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/reports/top-components", async (string? sortBy, int? limit, ISender sender) =>
        {
            var result = await sender.Send(new GetTopComponentsQuery(sortBy, limit ?? 10));
            return result.Match(r => Microsoft.AspNetCore.Http.Results.Ok(r), ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Reports);
    }
}
