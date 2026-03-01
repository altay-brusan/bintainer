using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Common.Presentation.Results;
using Bintainer.Modules.Users.Application.Users.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.Users.Presentation.Users;

internal sealed class RegisterUser : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/auth/register", async (RegisterUserRequest request, ISender sender) =>
        {
            var command = new RegisterUserCommand(request.Email, request.Password, request.FirstName, request.LastName);

            var result = await sender.Send(command);

            return result.Match(
                userId => Microsoft.AspNetCore.Http.Results.Ok(userId),
                ApiResults.Problem);
        })
        .AllowAnonymous()
        .WithTags(Tags.Users);
    }

    internal sealed record RegisterUserRequest(string Email, string Password, string FirstName, string LastName);
}
