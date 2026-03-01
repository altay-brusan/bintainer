using Bintainer.Common.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bintainer.Common.Application.UnitTests.Behaviors;

public class ExceptionHandlingPipelineBehaviorTests
{
    [Fact]
    public async Task Handle_NoException_ReturnsResult()
    {
        var logger = NullLogger<ExceptionHandlingPipelineBehavior<TestCommand, string>>.Instance;
        var behavior = new ExceptionHandlingPipelineBehavior<TestCommand, string>(logger);
        RequestHandlerDelegate<string> next = () => Task.FromResult("success");

        var result = await behavior.Handle(new TestCommand("test"), next, CancellationToken.None);

        result.Should().Be("success");
    }

    [Fact]
    public async Task Handle_Exception_LogsAndRethrows()
    {
        var logger = NullLogger<ExceptionHandlingPipelineBehavior<TestCommand, string>>.Instance;
        var behavior = new ExceptionHandlingPipelineBehavior<TestCommand, string>(logger);
        RequestHandlerDelegate<string> next = () => throw new InvalidOperationException("test error");

        var act = () => behavior.Handle(new TestCommand("test"), next, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("test error");
    }
}
