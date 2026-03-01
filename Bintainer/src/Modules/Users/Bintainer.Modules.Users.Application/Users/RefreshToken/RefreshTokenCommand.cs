using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Users.Application.Users.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<RefreshTokenResponse>;

public sealed record RefreshTokenResponse(string AccessToken, string RefreshToken);
