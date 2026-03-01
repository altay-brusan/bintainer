using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Users.Application.Users.LogoutUser;

public sealed record LogoutUserCommand(Guid UserId) : ICommand;
