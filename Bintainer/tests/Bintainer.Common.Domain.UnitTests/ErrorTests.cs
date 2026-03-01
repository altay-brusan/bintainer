using Bintainer.Common.Domain;

namespace Bintainer.Common.Domain.UnitTests;

public class ErrorTests
{
    [Fact]
    public void None_HasEmptyCodeAndDescription()
    {
        Error.None.Code.Should().BeEmpty();
        Error.None.Description.Should().BeEmpty();
        Error.None.Type.Should().Be(ErrorType.Failure);
    }

    [Fact]
    public void Failure_CreatesFailureError()
    {
        var error = Error.Failure("code", "description");

        error.Code.Should().Be("code");
        error.Description.Should().Be("description");
        error.Type.Should().Be(ErrorType.Failure);
    }

    [Fact]
    public void NotFound_CreatesNotFoundError()
    {
        var error = Error.NotFound("code", "description");

        error.Code.Should().Be("code");
        error.Description.Should().Be("description");
        error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public void Problem_CreatesProblemError()
    {
        var error = Error.Problem("code", "description");

        error.Code.Should().Be("code");
        error.Description.Should().Be("description");
        error.Type.Should().Be(ErrorType.Problem);
    }

    [Fact]
    public void Conflict_CreatesConflictError()
    {
        var error = Error.Conflict("code", "description");

        error.Code.Should().Be("code");
        error.Description.Should().Be("description");
        error.Type.Should().Be(ErrorType.Conflict);
    }
}
