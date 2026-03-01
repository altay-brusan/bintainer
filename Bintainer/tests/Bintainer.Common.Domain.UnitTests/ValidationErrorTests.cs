using Bintainer.Common.Domain;

namespace Bintainer.Common.Domain.UnitTests;

public class ValidationErrorTests
{
    [Fact]
    public void Constructor_SetsErrorsAndDefaults()
    {
        var errors = new[] { Error.Failure("e1", "error 1"), Error.Failure("e2", "error 2") };

        var validationError = new ValidationError(errors);

        validationError.Code.Should().Be("General.Validation");
        validationError.Description.Should().Be("One or more validation errors occurred.");
        validationError.Type.Should().Be(ErrorType.Validation);
        validationError.Errors.Should().HaveCount(2);
    }

    [Fact]
    public void FromResults_FiltersOnlyFailures()
    {
        var results = new[]
        {
            Result.Success(),
            Result.Failure(Error.Failure("e1", "error 1")),
            Result.Success(),
            Result.Failure(Error.NotFound("e2", "error 2"))
        };

        var validationError = ValidationError.FromResults(results);

        validationError.Errors.Should().HaveCount(2);
        validationError.Errors[0].Code.Should().Be("e1");
        validationError.Errors[1].Code.Should().Be("e2");
    }
}
