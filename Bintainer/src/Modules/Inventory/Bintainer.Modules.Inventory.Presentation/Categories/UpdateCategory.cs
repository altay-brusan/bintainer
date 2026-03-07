using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Inventory.Application.Categories.UpdateCategory;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Inventory.Presentation.Categories;

internal sealed class UpdateCategory : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("api/categories/{categoryId:guid}", async (Guid categoryId, UpdateCategoryRequest request, ISender sender) =>
        {
            var command = new UpdateCategoryCommand(categoryId, request.Name, request.ParentId);

            var result = await sender.Send(command);

            return result.Match(
                () => Microsoft.AspNetCore.Http.Results.NoContent(),
                ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Categories);
    }

    internal sealed record UpdateCategoryRequest(string Name, Guid? ParentId);
}
