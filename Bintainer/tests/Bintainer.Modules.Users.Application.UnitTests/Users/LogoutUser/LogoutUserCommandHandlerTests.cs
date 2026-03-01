using Bintainer.Modules.Users.Application.Abstractions.Data;
using Bintainer.Modules.Users.Application.Users.LogoutUser;
using Bintainer.Modules.Users.Domain.Users;

namespace Bintainer.Modules.Users.Application.UnitTests.Users.LogoutUser;

public class LogoutUserCommandHandlerTests
{
    private readonly IRefreshTokenRepository _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly LogoutUserCommandHandler _handler;

    public LogoutUserCommandHandlerTests()
    {
        _handler = new LogoutUserCommandHandler(_refreshTokenRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_CallsRevokeAllForUser()
    {
        var userId = Guid.NewGuid();
        var command = new LogoutUserCommand(userId);

        await _handler.Handle(command, CancellationToken.None);

        await _refreshTokenRepository.Received(1).RevokeAllForUserAsync(userId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CallsSaveChanges()
    {
        var command = new LogoutUserCommand(Guid.NewGuid());

        await _handler.Handle(command, CancellationToken.None);

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ReturnsSuccess()
    {
        var command = new LogoutUserCommand(Guid.NewGuid());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }
}
