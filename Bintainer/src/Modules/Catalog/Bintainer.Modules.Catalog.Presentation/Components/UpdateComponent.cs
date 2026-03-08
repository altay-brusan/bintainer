using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Catalog.Application.Components.UpdateComponent;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Catalog.Presentation.Components;

internal sealed class UpdateComponent : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("api/components/{componentId:guid}", async (Guid componentId, UpdateComponentRequest request, ISender sender) =>
        {
            var command = new UpdateComponentCommand(
                componentId,
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
                request.Tags,
                request.UnitPrice,
                request.Manufacturer,
                request.LowStockThreshold);

            var result = await sender.Send(command);

            return result.Match(
                () => Microsoft.AspNetCore.Http.Results.NoContent(),
                ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Components);
    }

    internal sealed record UpdateComponentRequest(
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
        string? Tags,
        decimal? UnitPrice,
        string? Manufacturer,
        int LowStockThreshold);
}
