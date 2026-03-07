using Bintainer.Common.Application.Authorization;
using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Users.Application.Users.UpdatePreferences;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Users.Presentation.Users;

internal sealed class UpdatePreferences : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("api/auth/preferences", async (UpdatePreferencesRequest request, ICurrentUserService currentUser, ISender sender) =>
        {
            var command = new UpdatePreferencesCommand(currentUser.UserId, request.Currency);

            var result = await sender.Send(command);

            return result.Match(
                () => Microsoft.AspNetCore.Http.Results.NoContent(),
                ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Users);
    }

    internal sealed record UpdatePreferencesRequest(string Currency);
}
