using Bintainer.Modules.Catalog.Application.Components.UpdateComponent;
using FluentValidation.TestHelper;

namespace Bintainer.Modules.Catalog.Application.UnitTests.Components.UpdateComponent;

public class UpdateComponentCommandValidatorTests
{
    private readonly UpdateComponentCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_NoErrors()
    {
        var command = new UpdateComponentCommand(
            Guid.NewGuid(), "PN", "MPN", "Desc", null, null, null, null, null, null, null, null, null, null, null, 0);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyComponentId_HasError()
    {
        var command = new UpdateComponentCommand(
            Guid.Empty, "PN", "MPN", "Desc", null, null, null, null, null, null, null, null, null, null, null, 0);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ComponentId);
    }

    [Fact]
    public void Validate_EmptyPartNumber_HasError()
    {
        var command = new UpdateComponentCommand(
            Guid.NewGuid(), "", "MPN", "Desc", null, null, null, null, null, null, null, null, null, null, null, 0);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PartNumber);
    }
}
