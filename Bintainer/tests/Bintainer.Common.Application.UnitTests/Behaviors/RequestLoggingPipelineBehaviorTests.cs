using Bintainer.Common.Application.Behaviors;
using Bintainer.Common.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bintainer.Common.Application.UnitTests.Behaviors;

public class RequestLoggingPipelineBehaviorTests
{
    [Fact]
    public async Task Handle_SuccessResult_ReturnsSuccess()
    {
        var logger = NullLogger<RequestLoggingPipelineBehavior<TestCommandNoResult, Result>>.Instance;
        var behavior = new RequestLoggingPipelineBehavior<TestCommandNoResult, Result>(logger);
        RequestHandlerDelegate<Result> next = () => Task.FromResult(Result.Success());

        var result = await behavior.Handle(new TestCommandNoResult("test"), next, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_FailureResult_ReturnsFailure()
    {
        var logger = NullLogger<RequestLoggingPipelineBehavior<TestCommandNoResult, Result>>.Instance;
        var behavior = new RequestLoggingPipelineBehavior<TestCommandNoResult, Result>(logger);
        RequestHandlerDelegate<Result> next = () => Task.FromResult(Result.Failure(Error.Failure("test", "test error")));

        var result = await behavior.Handle(new TestCommandNoResult("test"), next, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }
}
