using Bintainer.Common.Domain;
using Bintainer.Modules.Users.Application.Abstractions.Data;
using Bintainer.Modules.Users.Application.Abstractions.Identity;
using Bintainer.Modules.Users.Application.Users.RegisterUser;
using Bintainer.Modules.Users.Domain.Users;

namespace Bintainer.Modules.Users.Application.UnitTests.Users.RegisterUser;

public class RegisterUserCommandHandlerTests
{
    private readonly IIdentityService _identityService = Substitute.For<IIdentityService>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _handler = new RegisterUserCommandHandler(_identityService, _userRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccessWithUserId()
    {
        var command = new RegisterUserCommand("test@example.com", "Password1", "John", "Doe");
        _identityService.RegisterAsync(command.Email, command.Password)
            .Returns(Result.Success("identity-123"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_IdentityFailure_ReturnsFailure()
    {
        var command = new RegisterUserCommand("test@example.com", "Password1", "John", "Doe");
        _identityService.RegisterAsync(command.Email, command.Password)
            .Returns(Result.Failure<string>(UserErrors.DuplicateEmail));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.DuplicateEmail);
    }

    [Fact]
    public async Task Handle_Success_CallsInsertOnRepository()
    {
        var command = new RegisterUserCommand("test@example.com", "Password1", "John", "Doe");
        _identityService.RegisterAsync(command.Email, command.Password)
            .Returns(Result.Success("identity-123"));

        await _handler.Handle(command, CancellationToken.None);

        _userRepository.Received(1).Insert(Arg.Is<User>(u =>
            u.Email == "test@example.com" &&
            u.FirstName == "John" &&
            u.LastName == "Doe"));
    }

    [Fact]
    public async Task Handle_Success_CallsSaveChanges()
    {
        var command = new RegisterUserCommand("test@example.com", "Password1", "John", "Doe");
        _identityService.RegisterAsync(command.Email, command.Password)
            .Returns(Result.Success("identity-123"));

        await _handler.Handle(command, CancellationToken.None);

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_IdentityFailure_DoesNotCallRepository()
    {
        var command = new RegisterUserCommand("test@example.com", "Password1", "John", "Doe");
        _identityService.RegisterAsync(command.Email, command.Password)
            .Returns(Result.Failure<string>(UserErrors.DuplicateEmail));

        await _handler.Handle(command, CancellationToken.None);

        _userRepository.DidNotReceive().Insert(Arg.Any<User>());
    }
}
