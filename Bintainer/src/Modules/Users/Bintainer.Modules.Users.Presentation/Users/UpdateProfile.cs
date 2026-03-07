using Bintainer.Common.Application.Authorization;
using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Users.Application.Users.UpdateProfile;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Users.Presentation.Users;

internal sealed class UpdateProfile : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("api/auth/profile", async (UpdateProfileRequest request, ICurrentUserService currentUser, ISender sender) =>
        {
            var command = new UpdateProfileCommand(currentUser.UserId, request.FirstName, request.LastName);

            var result = await sender.Send(command);

            return result.Match(
                () => Microsoft.AspNetCore.Http.Results.NoContent(),
                ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Users);
    }

    internal sealed record UpdateProfileRequest(string FirstName, string LastName);
}
