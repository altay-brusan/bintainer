using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Inventory.Application.Reports.GetMovementTimeline;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Inventory.Presentation.Reports;

internal sealed class GetMovementTimeline : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/reports/movement-timeline", async (int? days, ISender sender) =>
        {
            var result = await sender.Send(new GetMovementTimelineQuery(days ?? 7));
            return result.Match(r => Microsoft.AspNetCore.Http.Results.Ok(r), ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Reports);
    }
}
