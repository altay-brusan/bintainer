using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Inventory.Application.Categories.CreateCategory;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Inventory.Presentation.Categories;

internal sealed class CreateCategory : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/categories", async (CreateCategoryRequest request, ISender sender) =>
        {
            var command = new CreateCategoryCommand(request.Name, request.ParentId);

            var result = await sender.Send(command);

            return result.Match(
                id => Microsoft.AspNetCore.Http.Results.Created($"/api/categories/{id}", id),
                ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Categories);
    }

    internal sealed record CreateCategoryRequest(string Name, Guid? ParentId);
}
