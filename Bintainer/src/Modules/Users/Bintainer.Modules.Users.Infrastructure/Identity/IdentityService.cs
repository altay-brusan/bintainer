using Bintainer.Common.Domain;
using Bintainer.Modules.Users.Application.Abstractions.Identity;
using Bintainer.Modules.Users.Domain.Users;
using Microsoft.AspNetCore.Identity;

namespace Bintainer.Modules.Users.Infrastructure.Identity;

internal sealed class IdentityService(UserManager<IdentityUser> userManager) : IIdentityService
{
    public async Task<Result<string>> RegisterAsync(string email, string password)
    {
        var identityUser = new IdentityUser
        {
            UserName = email,
            Email = email
        };

        IdentityResult result = await userManager.CreateAsync(identityUser, password);

        if (!result.Succeeded)
        {
            var error = result.Errors.First();

            return error.Code == "DuplicateEmail" || error.Code == "DuplicateUserName"
                ? Result.Failure<string>(UserErrors.DuplicateEmail)
                : Result.Failure<string>(Error.Problem("Identity.Registration", error.Description));
        }

        return identityUser.Id;
    }

    public async Task<Result<string>> LoginAsync(string email, string password)
    {
        IdentityUser? identityUser = await userManager.FindByEmailAsync(email);

        if (identityUser is null)
        {
            return Result.Failure<string>(UserErrors.InvalidCredentials);
        }

        bool isValid = await userManager.CheckPasswordAsync(identityUser, password);

        if (!isValid)
        {
            return Result.Failure<string>(UserErrors.InvalidCredentials);
        }

        return identityUser.Id;
    }
}
