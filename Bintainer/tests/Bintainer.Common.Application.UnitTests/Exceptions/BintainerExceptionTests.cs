using Bintainer.Common.Application.Exceptions;
using Bintainer.Common.Domain;

namespace Bintainer.Common.Application.UnitTests.Exceptions;

public class BintainerExceptionTests
{
    [Fact]
    public void Constructor_SetsRequestName()
    {
        var exception = new BintainerException("TestRequest");

        exception.RequestName.Should().Be("TestRequest");
        exception.Error.Should().BeNull();
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithError_SetsError()
    {
        var error = Error.Failure("code", "desc");

        var exception = new BintainerException("TestRequest", error);

        exception.RequestName.Should().Be("TestRequest");
        exception.Error.Should().Be(error);
    }

    [Fact]
    public void Constructor_WithInnerException_SetsInnerException()
    {
        var inner = new InvalidOperationException("inner");

        var exception = new BintainerException("TestRequest", innerException: inner);

        exception.InnerException.Should().Be(inner);
    }

    [Fact]
    public void Message_IsApplicationException()
    {
        var exception = new BintainerException("TestRequest");

        exception.Message.Should().Be("Application exception");
    }
}
