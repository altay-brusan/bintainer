using Bintainer.Common.Domain;

namespace Bintainer.Modules.Users.Domain.Users;

public sealed class RefreshToken : Entity
{
    private RefreshToken() { }

    public string Token { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }
    public DateTime ExpiresOnUtc { get; private set; }
    public bool IsRevoked { get; private set; }

    public static RefreshToken Create(string token, Guid userId, DateTime expiresOnUtc)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = token,
            UserId = userId,
            ExpiresOnUtc = expiresOnUtc,
            IsRevoked = false
        };
    }

    public void Revoke()
    {
        IsRevoked = true;
    }
}
