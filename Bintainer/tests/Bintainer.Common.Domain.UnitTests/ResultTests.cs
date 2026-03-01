using Bintainer.Common.Domain;

namespace Bintainer.Common.Domain.UnitTests;

public class ResultTests
{
    [Fact]
    public void Success_ReturnsSuccessResult()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
    }

    [Fact]
    public void Failure_ReturnsFailureResult()
    {
        var error = Error.Failure("test", "test error");

        var result = Result.Failure(error);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void SuccessT_ReturnsSuccessWithValue()
    {
        var result = Result.Success(42);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void FailureT_ReturnsFailureResult()
    {
        var error = Error.NotFound("test", "not found");

        var result = Result.Failure<int>(error);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void Value_OnFailure_ThrowsInvalidOperationException()
    {
        var result = Result.Failure<int>(Error.Failure("test", "fail"));

        var act = () => result.Value;

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ImplicitConversion_FromValue_ReturnsSuccess()
    {
        Result<string> result = "hello";

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("hello");
    }

    [Fact]
    public void ImplicitConversion_FromNull_ReturnsNullValueFailure()
    {
        Result<string> result = (string)null!;

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.NullValue);
    }

    [Fact]
    public void Constructor_SuccessWithError_ThrowsArgumentException()
    {
        var act = () => Result.Failure(Error.None);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Success_ErrorIsNone()
    {
        var result = Result.Success();

        result.Error.Should().Be(Error.None);
    }
}
