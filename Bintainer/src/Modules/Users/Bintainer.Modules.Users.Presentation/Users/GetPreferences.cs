using Bintainer.Common.Application.Authorization;
using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Users.Application.Users.GetPreferences;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Users.Presentation.Users;

internal sealed class GetPreferences : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/auth/preferences", async (ICurrentUserService currentUser, ISender sender) =>
        {
            var query = new GetPreferencesQuery(currentUser.UserId);

            var result = await sender.Send(query);

            return result.Match(
                response => Microsoft.AspNetCore.Http.Results.Ok(response),
                ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Users);
    }
}
