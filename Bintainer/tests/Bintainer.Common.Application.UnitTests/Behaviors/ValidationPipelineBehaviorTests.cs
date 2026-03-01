using Bintainer.Common.Application.Behaviors;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using FluentValidation;
using MediatR;

namespace Bintainer.Common.Application.UnitTests.Behaviors;

public sealed record TestCommand(string Name) : ICommand<string>;

public sealed record TestCommandNoResult(string Name) : ICommand;

public class ValidationPipelineBehaviorTests
{
    [Fact]
    public async Task Handle_NoValidators_CallsNext()
    {
        var behavior = new ValidationPipelineBehavior<TestCommand, Result<string>>(
            Enumerable.Empty<IValidator<TestCommand>>());
        RequestHandlerDelegate<Result<string>> next = () => Task.FromResult(Result.Success("ok"));

        var result = await behavior.Handle(new TestCommand("valid"), next, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("ok");
    }

    [Fact]
    public async Task Handle_ValidRequest_CallsNext()
    {
        var validator = new AlwaysValidValidator();
        var behavior = new ValidationPipelineBehavior<TestCommand, Result<string>>(new[] { validator });
        RequestHandlerDelegate<Result<string>> next = () => Task.FromResult(Result.Success("ok"));

        var result = await behavior.Handle(new TestCommand("valid"), next, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_InvalidRequest_ResultT_ReturnsValidationError()
    {
        var validator = new AlwaysInvalidValidator();
        var behavior = new ValidationPipelineBehavior<TestCommand, Result<string>>(new[] { validator });
        RequestHandlerDelegate<Result<string>> next = () => Task.FromResult(Result.Success("ok"));

        var result = await behavior.Handle(new TestCommand(""), next, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ValidationError>();
    }

    [Fact]
    public async Task Handle_InvalidRequest_Result_ReturnsValidationError()
    {
        var validator = new AlwaysInvalidNoResultValidator();
        var behavior = new ValidationPipelineBehavior<TestCommandNoResult, Result>(new[] { validator });
        RequestHandlerDelegate<Result> next = () => Task.FromResult(Result.Success());

        var result = await behavior.Handle(new TestCommandNoResult(""), next, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ValidationError>();
    }

    [Fact]
    public async Task Handle_InvalidRequest_NonResultType_ThrowsValidationException()
    {
        var validator = new AlwaysInvalidValidator();
        var behavior = new ValidationPipelineBehavior<TestCommand, string>(new[] { validator });
        RequestHandlerDelegate<string> next = () => Task.FromResult("ok");

        var act = () => behavior.Handle(new TestCommand(""), next, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_MultipleValidators_AggregatesErrors()
    {
        var validator1 = new NameRequiredValidator();
        var validator2 = new NameMinLengthValidator();
        var behavior = new ValidationPipelineBehavior<TestCommand, Result<string>>(
            new IValidator<TestCommand>[] { validator1, validator2 });
        RequestHandlerDelegate<Result<string>> next = () => Task.FromResult(Result.Success("ok"));

        var result = await behavior.Handle(new TestCommand(""), next, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        var validationError = result.Error.Should().BeOfType<ValidationError>().Subject;
        validationError.Errors.Should().HaveCount(2);
    }

    private sealed class AlwaysValidValidator : AbstractValidator<TestCommand> { }

    private sealed class AlwaysInvalidValidator : AbstractValidator<TestCommand>
    {
        public AlwaysInvalidValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    private sealed class AlwaysInvalidNoResultValidator : AbstractValidator<TestCommandNoResult>
    {
        public AlwaysInvalidNoResultValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    private sealed class NameRequiredValidator : AbstractValidator<TestCommand>
    {
        public NameRequiredValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    private sealed class NameMinLengthValidator : AbstractValidator<TestCommand>
    {
        public NameMinLengthValidator()
        {
            RuleFor(x => x.Name).MinimumLength(5);
        }
    }
}
