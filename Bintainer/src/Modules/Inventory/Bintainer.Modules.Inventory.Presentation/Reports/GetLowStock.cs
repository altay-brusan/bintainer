using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Inventory.Application.Reports.GetLowStock;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Inventory.Presentation.Reports;

internal sealed class GetLowStock : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/reports/low-stock", async (ISender sender) =>
        {
            var result = await sender.Send(new GetLowStockQuery());
            return result.Match(r => Microsoft.AspNetCore.Http.Results.Ok(r), ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Reports);
    }
}
