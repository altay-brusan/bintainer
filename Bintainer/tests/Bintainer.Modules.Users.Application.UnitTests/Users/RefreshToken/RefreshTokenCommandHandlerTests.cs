using Bintainer.Common.Application.Clock;
using Bintainer.Common.Domain;
using Bintainer.Modules.Users.Application.Abstractions.Data;
using Bintainer.Modules.Users.Application.Abstractions.Identity;
using Bintainer.Modules.Users.Application.Users.RefreshToken;
using Bintainer.Modules.Users.Domain.Users;

namespace Bintainer.Modules.Users.Application.UnitTests.Users.RefreshToken;

public class RefreshTokenCommandHandlerTests
{
    private readonly IRefreshTokenRepository _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IJwtTokenGenerator _jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _dateTimeProvider.UtcNow.Returns(new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc));
        _handler = new RefreshTokenCommandHandler(
            _refreshTokenRepository, _userRepository, _jwtTokenGenerator,
            _dateTimeProvider, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ValidToken_ReturnsNewTokens()
    {
        var userId = Guid.NewGuid();
        var existingToken = Domain.Users.RefreshToken.Create("old-token", userId, DateTime.UtcNow.AddDays(7));
        var user = User.Create("test@example.com", "John", "Doe", "identity-123");

        _refreshTokenRepository.GetByTokenAsync("old-token", Arg.Any<CancellationToken>())
            .Returns(existingToken);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);
        _jwtTokenGenerator.GenerateAccessToken(user).Returns("new-access-token");
        _jwtTokenGenerator.GenerateRefreshToken().Returns("new-refresh-token");

        var command = new RefreshTokenCommand("old-token");
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be("new-access-token");
        result.Value.RefreshToken.Should().Be("new-refresh-token");
    }

    [Fact]
    public async Task Handle_ValidToken_RevokesOldToken()
    {
        var userId = Guid.NewGuid();
        var existingToken = Domain.Users.RefreshToken.Create("old-token", userId, DateTime.UtcNow.AddDays(7));
        var user = User.Create("test@example.com", "John", "Doe", "identity-123");

        _refreshTokenRepository.GetByTokenAsync("old-token", Arg.Any<CancellationToken>())
            .Returns(existingToken);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);
        _jwtTokenGenerator.GenerateAccessToken(user).Returns("at");
        _jwtTokenGenerator.GenerateRefreshToken().Returns("rt");

        var command = new RefreshTokenCommand("old-token");
        await _handler.Handle(command, CancellationToken.None);

        existingToken.IsRevoked.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_TokenNotFound_ReturnsInvalidCredentials()
    {
        _refreshTokenRepository.GetByTokenAsync("missing-token", Arg.Any<CancellationToken>())
            .Returns((Domain.Users.RefreshToken?)null);

        var command = new RefreshTokenCommand("missing-token");
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_RevokedToken_ReturnsInvalidCredentials()
    {
        var existingToken = Domain.Users.RefreshToken.Create("revoked-token", Guid.NewGuid(), DateTime.UtcNow.AddDays(7));
        existingToken.Revoke();

        _refreshTokenRepository.GetByTokenAsync("revoked-token", Arg.Any<CancellationToken>())
            .Returns(existingToken);

        var command = new RefreshTokenCommand("revoked-token");
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_ExpiredToken_ReturnsInvalidCredentials()
    {
        var now = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        _dateTimeProvider.UtcNow.Returns(now);

        var existingToken = Domain.Users.RefreshToken.Create("expired-token", Guid.NewGuid(), now.AddDays(-1));

        _refreshTokenRepository.GetByTokenAsync("expired-token", Arg.Any<CancellationToken>())
            .Returns(existingToken);

        var command = new RefreshTokenCommand("expired-token");
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsNotFound()
    {
        var userId = Guid.NewGuid();
        var existingToken = Domain.Users.RefreshToken.Create("token", userId, DateTime.UtcNow.AddDays(7));

        _refreshTokenRepository.GetByTokenAsync("token", Arg.Any<CancellationToken>())
            .Returns(existingToken);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var command = new RefreshTokenCommand("token");
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Users.NotFound");
    }

    [Fact]
    public async Task Handle_Success_InsertsNewRefreshToken()
    {
        var userId = Guid.NewGuid();
        var existingToken = Domain.Users.RefreshToken.Create("old-token", userId, DateTime.UtcNow.AddDays(7));
        var user = User.Create("test@example.com", "John", "Doe", "identity-123");

        _refreshTokenRepository.GetByTokenAsync("old-token", Arg.Any<CancellationToken>())
            .Returns(existingToken);
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);
        _jwtTokenGenerator.GenerateAccessToken(user).Returns("at");
        _jwtTokenGenerator.GenerateRefreshToken().Returns("new-rt");

        var command = new RefreshTokenCommand("old-token");
        await _handler.Handle(command, CancellationToken.None);

        _refreshTokenRepository.Received(1).Insert(Arg.Is<Domain.Users.RefreshToken>(rt =>
            rt.Token == "new-rt" && rt.UserId == user.Id));
    }
}
