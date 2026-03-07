using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Inventory.Application.Parts.CreatePart;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Inventory.Presentation.Parts;

internal sealed class CreatePart : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/parts", async (CreatePartRequest request, ISender sender) =>
        {
            var command = new CreatePartCommand(
                request.PartNumber,
                request.ManufacturerPartNumber,
                request.Description,
                request.DetailedDescription,
                request.ImageUrl,
                request.Url,
                request.Provider,
                request.ProviderPartNumber,
                request.CategoryId,
                request.FootprintId,
                request.Attributes,
                request.Tags);

            var result = await sender.Send(command);

            return result.Match(
                id => Microsoft.AspNetCore.Http.Results.Created($"/api/parts/{id}", id),
                ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Parts);
    }

    internal sealed record CreatePartRequest(
        string PartNumber,
        string ManufacturerPartNumber,
        string Description,
        string? DetailedDescription,
        string? ImageUrl,
        string? Url,
        string? Provider,
        string? ProviderPartNumber,
        Guid? CategoryId,
        Guid? FootprintId,
        Dictionary<string, string>? Attributes,
        string? Tags);
}
