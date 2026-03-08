using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Catalog.Application.Components.UploadComponentImage;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Catalog.Presentation.Components;

internal sealed class UploadComponentImage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/components/{componentId:guid}/image", async (Guid componentId, IFormFile file, ISender sender) =>
        {
            using var stream = file.OpenReadStream();
            var command = new UploadComponentImageCommand(componentId, stream, file.FileName, file.ContentType);

            var result = await sender.Send(command);

            return result.Match(
                url => Microsoft.AspNetCore.Http.Results.Ok(new { imageUrl = url }),
                ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Components)
        .DisableAntiforgery();
    }
}
