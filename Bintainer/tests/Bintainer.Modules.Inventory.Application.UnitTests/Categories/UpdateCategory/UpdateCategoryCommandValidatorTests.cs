using Bintainer.Modules.Inventory.Application.Categories.UpdateCategory;
using FluentValidation.TestHelper;

namespace Bintainer.Modules.Inventory.Application.UnitTests.Categories.UpdateCategory;

public class UpdateCategoryCommandValidatorTests
{
    private readonly UpdateCategoryCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_NoErrors()
    {
        var command = new UpdateCategoryCommand(Guid.NewGuid(), "Electronics", null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyCategoryId_HasError()
    {
        var command = new UpdateCategoryCommand(Guid.Empty, "Electronics", null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }

    [Fact]
    public void Validate_EmptyName_HasError()
    {
        var command = new UpdateCategoryCommand(Guid.NewGuid(), "", null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}
