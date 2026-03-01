using Bintainer.Modules.Users.Domain.Users;

namespace Bintainer.Modules.Users.Application.Abstractions.Identity;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
