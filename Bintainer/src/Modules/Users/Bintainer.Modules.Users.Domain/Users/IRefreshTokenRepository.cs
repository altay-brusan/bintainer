namespace Bintainer.Modules.Users.Domain.Users;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    void Insert(RefreshToken refreshToken);
    Task RevokeAllForUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
