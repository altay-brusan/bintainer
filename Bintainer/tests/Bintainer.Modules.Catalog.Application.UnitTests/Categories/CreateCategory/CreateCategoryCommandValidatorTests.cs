using Bintainer.Modules.Catalog.Application.Categories.CreateCategory;
using FluentValidation.TestHelper;

namespace Bintainer.Modules.Catalog.Application.UnitTests.Categories.CreateCategory;

public class CreateCategoryCommandValidatorTests
{
    private readonly CreateCategoryCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_NoErrors()
    {
        var command = new CreateCategoryCommand("Electronics", null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyName_HasError()
    {
        var command = new CreateCategoryCommand("", null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_NameTooLong_HasError()
    {
        var command = new CreateCategoryCommand(new string('A', 201), null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}
