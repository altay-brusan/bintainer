using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Inventory.Application.BomImports.ImportBom;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Inventory.Presentation.BomImports;

internal sealed class ImportBom : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/bom/import", async (ImportBomRequest request, ISender sender) =>
        {
            var command = new ImportBomCommand(request.FileName, request.Lines);

            var result = await sender.Send(command);

            return result.Match(
                response => Microsoft.AspNetCore.Http.Results.Ok(response),
                ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.BomImports);
    }

    internal sealed record ImportBomRequest(string FileName, List<BomLineItem> Lines);
}
