using Bintainer.Common.Application.Clock;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Users.Application.Abstractions.Data;
using Bintainer.Modules.Users.Application.Abstractions.Identity;
using Bintainer.Modules.Users.Domain.Users;

namespace Bintainer.Modules.Users.Application.Users.LoginUser;

internal sealed class LoginUserCommandHandler(
    IIdentityService identityService,
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IJwtTokenGenerator jwtTokenGenerator,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<LoginUserCommand, LoginResponse>
{
    public async Task<Result<LoginResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        Result<string> loginResult = await identityService.LoginAsync(request.Email, request.Password);

        if (loginResult.IsFailure)
        {
            return Result.Failure<LoginResponse>(UserErrors.InvalidCredentials);
        }

        string identityId = loginResult.Value;

        User? user = await userRepository.GetByIdentityIdAsync(identityId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<LoginResponse>(UserErrors.NotFoundByIdentityId(identityId));
        }

        string accessToken = jwtTokenGenerator.GenerateAccessToken(user);
        string refreshTokenValue = jwtTokenGenerator.GenerateRefreshToken();

        var refreshToken = Domain.Users.RefreshToken.Create(
            refreshTokenValue,
            user.Id,
            dateTimeProvider.UtcNow.AddDays(7));

        refreshTokenRepository.Insert(refreshToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new LoginResponse(accessToken, refreshTokenValue);
    }
}
