using Bintainer.Common.Domain;

namespace Bintainer.Modules.Users.Domain.Users;

public static class UserErrors
{
    public static Error NotFound(Guid userId) =>
        Error.NotFound("Users.NotFound", $"The user with Id '{userId}' was not found.");

    public static Error NotFoundByIdentityId(string identityId) =>
        Error.NotFound("Users.NotFoundByIdentityId", $"The user with identity Id '{identityId}' was not found.");

    public static readonly Error InvalidCredentials =
        Error.Problem("Users.InvalidCredentials", "The provided credentials are invalid.");

    public static readonly Error DuplicateEmail =
        Error.Conflict("Users.DuplicateEmail", "The provided email is already in use.");
}
