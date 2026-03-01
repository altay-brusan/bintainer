using Bintainer.Modules.Users.Application.Users.RegisterUser;
using FluentValidation.TestHelper;

namespace Bintainer.Modules.Users.Application.UnitTests.Users.RegisterUser;

public class RegisterUserCommandValidatorTests
{
    private readonly RegisterUserCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_NoErrors()
    {
        var command = new RegisterUserCommand("test@example.com", "Password1", "John", "Doe");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyEmail_HasError()
    {
        var command = new RegisterUserCommand("", "Password1", "John", "Doe");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_InvalidEmail_HasError()
    {
        var command = new RegisterUserCommand("not-an-email", "Password1", "John", "Doe");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_EmptyPassword_HasError()
    {
        var command = new RegisterUserCommand("test@example.com", "", "John", "Doe");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Validate_ShortPassword_HasError()
    {
        var command = new RegisterUserCommand("test@example.com", "12345", "John", "Doe");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Validate_EmptyFirstName_HasError()
    {
        var command = new RegisterUserCommand("test@example.com", "Password1", "", "Doe");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Validate_EmptyLastName_HasError()
    {
        var command = new RegisterUserCommand("test@example.com", "Password1", "John", "");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }
}
