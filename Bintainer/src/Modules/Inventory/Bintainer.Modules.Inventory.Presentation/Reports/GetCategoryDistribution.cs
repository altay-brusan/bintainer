using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Inventory.Application.Reports.GetCategoryDistribution;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Inventory.Presentation.Reports;

internal sealed class GetCategoryDistribution : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/reports/category-distribution", async (ISender sender) =>
        {
            var result = await sender.Send(new GetCategoryDistributionQuery());
            return result.Match(r => Microsoft.AspNetCore.Http.Results.Ok(r), ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Reports);
    }
}
