using Bintainer.Common.Application.Clock;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Users.Application.Abstractions.Data;
using Bintainer.Modules.Users.Application.Abstractions.Identity;
using Bintainer.Modules.Users.Domain.Users;

namespace Bintainer.Modules.Users.Application.Users.RefreshToken;

internal sealed class RefreshTokenCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository,
    IJwtTokenGenerator jwtTokenGenerator,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    public async Task<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        Domain.Users.RefreshToken? existingToken = await refreshTokenRepository.GetByTokenAsync(
            request.RefreshToken, cancellationToken);

        if (existingToken is null || existingToken.IsRevoked || existingToken.ExpiresOnUtc <= dateTimeProvider.UtcNow)
        {
            return Result.Failure<RefreshTokenResponse>(UserErrors.InvalidCredentials);
        }

        User? user = await userRepository.GetByIdAsync(existingToken.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<RefreshTokenResponse>(UserErrors.NotFound(existingToken.UserId));
        }

        existingToken.Revoke();

        string accessToken = jwtTokenGenerator.GenerateAccessToken(user);
        string newRefreshTokenValue = jwtTokenGenerator.GenerateRefreshToken();

        var newRefreshToken = Domain.Users.RefreshToken.Create(
            newRefreshTokenValue,
            user.Id,
            dateTimeProvider.UtcNow.AddDays(7));

        refreshTokenRepository.Insert(newRefreshToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new RefreshTokenResponse(accessToken, newRefreshTokenValue);
    }
}
