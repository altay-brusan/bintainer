using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Users.Application.Users.LoginUser;

public sealed record LoginUserCommand(string Email, string Password) : ICommand<LoginResponse>;

public sealed record LoginResponse(string AccessToken, string RefreshToken);
