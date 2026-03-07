using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Inventory.Application.Components.CreateComponent;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Inventory.Presentation.Components;

internal sealed class CreateComponent : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/components", async (CreateComponentRequest request, ISender sender) =>
        {
            var command = new CreateComponentCommand(
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
                id => Microsoft.AspNetCore.Http.Results.Created($"/api/components/{id}", id),
                ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Components);
    }

    internal sealed record CreateComponentRequest(
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
