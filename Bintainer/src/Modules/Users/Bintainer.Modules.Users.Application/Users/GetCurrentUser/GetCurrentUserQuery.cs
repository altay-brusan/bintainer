using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Users.Application.Users.GetCurrentUser;

public sealed record GetCurrentUserQuery(Guid UserId) : IQuery<CurrentUserResponse>;

public sealed record CurrentUserResponse(Guid Id, string Email, string FirstName, string LastName);
