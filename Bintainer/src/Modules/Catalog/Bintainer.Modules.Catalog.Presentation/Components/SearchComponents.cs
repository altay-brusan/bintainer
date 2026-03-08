using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Catalog.Application.Components.SearchComponents;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Catalog.Presentation.Components;

internal sealed class SearchComponents : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/components/search", async (string? q, Guid? categoryId, string? provider, string? tag, Guid? footprintId, int? page, int? pageSize, ISender sender) =>
        {
            var query = new SearchComponentsQuery(q, categoryId, provider, tag, footprintId, page ?? 1, pageSize ?? 20);

            var result = await sender.Send(query);

            return result.Match(
                response => Microsoft.AspNetCore.Http.Results.Ok(response),
                ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Components);
    }
}
