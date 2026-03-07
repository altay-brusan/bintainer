using Bintainer.Modules.Inventory.Application.Parts.UpdatePart;
using FluentValidation.TestHelper;

namespace Bintainer.Modules.Inventory.Application.UnitTests.Parts.UpdatePart;

public class UpdatePartCommandValidatorTests
{
    private readonly UpdatePartCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_NoErrors()
    {
        var command = new UpdatePartCommand(
            Guid.NewGuid(), "PN", "MPN", "Desc", null, null, null, null, null, null, null, null, null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyPartId_HasError()
    {
        var command = new UpdatePartCommand(
            Guid.Empty, "PN", "MPN", "Desc", null, null, null, null, null, null, null, null, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PartId);
    }

    [Fact]
    public void Validate_EmptyPartNumber_HasError()
    {
        var command = new UpdatePartCommand(
            Guid.NewGuid(), "", "MPN", "Desc", null, null, null, null, null, null, null, null, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PartNumber);
    }
}
