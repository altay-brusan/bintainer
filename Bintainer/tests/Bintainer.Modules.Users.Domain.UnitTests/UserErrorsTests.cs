using Bintainer.Common.Domain;
using Bintainer.Modules.Users.Domain.Users;

namespace Bintainer.Modules.Users.Domain.UnitTests;

public class UserErrorsTests
{
    [Fact]
    public void NotFound_ReturnsNotFoundError()
    {
        var userId = Guid.NewGuid();

        var error = UserErrors.NotFound(userId);

        error.Type.Should().Be(ErrorType.NotFound);
        error.Code.Should().Be("Users.NotFound");
        error.Description.Should().Contain(userId.ToString());
    }

    [Fact]
    public void NotFoundByIdentityId_ReturnsNotFoundError()
    {
        var error = UserErrors.NotFoundByIdentityId("identity-123");

        error.Type.Should().Be(ErrorType.NotFound);
        error.Code.Should().Be("Users.NotFoundByIdentityId");
        error.Description.Should().Contain("identity-123");
    }

    [Fact]
    public void InvalidCredentials_ReturnsProblemError()
    {
        var error = UserErrors.InvalidCredentials;

        error.Type.Should().Be(ErrorType.Problem);
        error.Code.Should().Be("Users.InvalidCredentials");
    }

    [Fact]
    public void DuplicateEmail_ReturnsConflictError()
    {
        var error = UserErrors.DuplicateEmail;

        error.Type.Should().Be(ErrorType.Conflict);
        error.Code.Should().Be("Users.DuplicateEmail");
    }
}
