using Bintainer.Modules.Users.Domain.Users;

namespace Bintainer.Modules.Users.Domain.UnitTests;

public class RefreshTokenTests
{
    [Fact]
    public void Create_ReturnsTokenWithProperties()
    {
        var userId = Guid.NewGuid();
        var expiresOn = DateTime.UtcNow.AddDays(7);

        var token = RefreshToken.Create("token-value", userId, expiresOn);

        token.Token.Should().Be("token-value");
        token.UserId.Should().Be(userId);
        token.ExpiresOnUtc.Should().Be(expiresOn);
        token.IsRevoked.Should().BeFalse();
    }

    [Fact]
    public void Create_GeneratesNewId()
    {
        var token = RefreshToken.Create("token", Guid.NewGuid(), DateTime.UtcNow.AddDays(7));

        token.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Revoke_SetsIsRevokedToTrue()
    {
        var token = RefreshToken.Create("token", Guid.NewGuid(), DateTime.UtcNow.AddDays(7));

        token.Revoke();

        token.IsRevoked.Should().BeTrue();
    }

    [Fact]
    public void Revoke_CalledTwice_RemainsRevoked()
    {
        var token = RefreshToken.Create("token", Guid.NewGuid(), DateTime.UtcNow.AddDays(7));

        token.Revoke();
        token.Revoke();

        token.IsRevoked.Should().BeTrue();
    }
}
