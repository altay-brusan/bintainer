using Bintainer.Common.Application.Clock;
using Bintainer.Common.Domain;
using Bintainer.Modules.Users.Application.Abstractions.Data;
using Bintainer.Modules.Users.Application.Abstractions.Identity;
using Bintainer.Modules.Users.Application.Users.LoginUser;
using Bintainer.Modules.Users.Domain.Users;

namespace Bintainer.Modules.Users.Application.UnitTests.Users.LoginUser;

public class LoginUserCommandHandlerTests
{
    private readonly IIdentityService _identityService = Substitute.For<IIdentityService>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IRefreshTokenRepository _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
    private readonly IJwtTokenGenerator _jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly LoginUserCommandHandler _handler;

    public LoginUserCommandHandlerTests()
    {
        _dateTimeProvider.UtcNow.Returns(new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc));
        _handler = new LoginUserCommandHandler(
            _identityService, _userRepository, _refreshTokenRepository,
            _jwtTokenGenerator, _dateTimeProvider, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsTokens()
    {
        var command = new LoginUserCommand("test@example.com", "Password1");
        var user = User.Create("test@example.com", "John", "Doe", "identity-123");

        _identityService.LoginAsync(command.Email, command.Password)
            .Returns(Result.Success("identity-123"));
        _userRepository.GetByIdentityIdAsync("identity-123", Arg.Any<CancellationToken>())
            .Returns(user);
        _jwtTokenGenerator.GenerateAccessToken(user).Returns("access-token");
        _jwtTokenGenerator.GenerateRefreshToken().Returns("refresh-token");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be("access-token");
        result.Value.RefreshToken.Should().Be("refresh-token");
    }

    [Fact]
    public async Task Handle_InvalidCredentials_ReturnsInvalidCredentials()
    {
        var command = new LoginUserCommand("test@example.com", "wrong");
        _identityService.LoginAsync(command.Email, command.Password)
            .Returns(Result.Failure<string>(UserErrors.InvalidCredentials));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsNotFoundByIdentityId()
    {
        var command = new LoginUserCommand("test@example.com", "Password1");
        _identityService.LoginAsync(command.Email, command.Password)
            .Returns(Result.Success("identity-123"));
        _userRepository.GetByIdentityIdAsync("identity-123", Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Users.NotFoundByIdentityId");
    }

    [Fact]
    public async Task Handle_Success_InsertsRefreshToken()
    {
        var command = new LoginUserCommand("test@example.com", "Password1");
        var user = User.Create("test@example.com", "John", "Doe", "identity-123");

        _identityService.LoginAsync(command.Email, command.Password)
            .Returns(Result.Success("identity-123"));
        _userRepository.GetByIdentityIdAsync("identity-123", Arg.Any<CancellationToken>())
            .Returns(user);
        _jwtTokenGenerator.GenerateAccessToken(user).Returns("access-token");
        _jwtTokenGenerator.GenerateRefreshToken().Returns("refresh-token");

        await _handler.Handle(command, CancellationToken.None);

        _refreshTokenRepository.Received(1).Insert(Arg.Is<Domain.Users.RefreshToken>(rt =>
            rt.Token == "refresh-token" && rt.UserId == user.Id));
    }

    [Fact]
    public async Task Handle_Success_RefreshTokenExpires7DaysFromNow()
    {
        var now = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        _dateTimeProvider.UtcNow.Returns(now);

        var command = new LoginUserCommand("test@example.com", "Password1");
        var user = User.Create("test@example.com", "John", "Doe", "identity-123");

        _identityService.LoginAsync(command.Email, command.Password)
            .Returns(Result.Success("identity-123"));
        _userRepository.GetByIdentityIdAsync("identity-123", Arg.Any<CancellationToken>())
            .Returns(user);
        _jwtTokenGenerator.GenerateAccessToken(user).Returns("access-token");
        _jwtTokenGenerator.GenerateRefreshToken().Returns("refresh-token");

        await _handler.Handle(command, CancellationToken.None);

        _refreshTokenRepository.Received(1).Insert(Arg.Is<Domain.Users.RefreshToken>(rt =>
            rt.ExpiresOnUtc == now.AddDays(7)));
    }

    [Fact]
    public async Task Handle_Success_CallsSaveChanges()
    {
        var command = new LoginUserCommand("test@example.com", "Password1");
        var user = User.Create("test@example.com", "John", "Doe", "identity-123");

        _identityService.LoginAsync(command.Email, command.Password)
            .Returns(Result.Success("identity-123"));
        _userRepository.GetByIdentityIdAsync("identity-123", Arg.Any<CancellationToken>())
            .Returns(user);
        _jwtTokenGenerator.GenerateAccessToken(user).Returns("at");
        _jwtTokenGenerator.GenerateRefreshToken().Returns("rt");

        await _handler.Handle(command, CancellationToken.None);

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
