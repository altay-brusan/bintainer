using Bintainer.Modules.Users.Application.Users.GetCurrentUser;
using Bintainer.Modules.Users.Domain.Users;

namespace Bintainer.Modules.Users.Application.UnitTests.Users.GetCurrentUser;

public class GetCurrentUserQueryHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly GetCurrentUserQueryHandler _handler;

    public GetCurrentUserQueryHandlerTests()
    {
        _handler = new GetCurrentUserQueryHandler(_userRepository);
    }

    [Fact]
    public async Task Handle_UserFound_ReturnsMappedResponse()
    {
        var user = User.Create("test@example.com", "John", "Doe", "identity-123");
        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>())
            .Returns(user);

        var query = new GetCurrentUserQuery(user.Id);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(user.Id);
        result.Value.Email.Should().Be("test@example.com");
        result.Value.FirstName.Should().Be("John");
        result.Value.LastName.Should().Be("Doe");
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsFailure()
    {
        var userId = Guid.NewGuid();
        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var query = new GetCurrentUserQuery(userId);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Users.NotFound");
    }

    [Fact]
    public async Task Handle_UserFound_AllPropertiesMapped()
    {
        var user = User.Create("jane@example.com", "Jane", "Smith", "id-456");
        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>())
            .Returns(user);

        var query = new GetCurrentUserQuery(user.Id);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Value.Should().BeEquivalentTo(new CurrentUserResponse(
            user.Id, "jane@example.com", "Jane", "Smith"));
    }
}
