using Bintainer.Modules.Inventory.Application.Parts.CreatePart;
using FluentValidation.TestHelper;

namespace Bintainer.Modules.Inventory.Application.UnitTests.Parts.CreatePart;

public class CreatePartCommandValidatorTests
{
    private readonly CreatePartCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_NoErrors()
    {
        var command = new CreatePartCommand(
            "PN-001", "MPN-001", "Resistor", null, null, null, null, null, null, null, null, null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyPartNumber_HasError()
    {
        var command = new CreatePartCommand(
            "", "MPN", "Desc", null, null, null, null, null, null, null, null, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PartNumber);
    }

    [Fact]
    public void Validate_PartNumberTooLong_HasError()
    {
        var command = new CreatePartCommand(
            new string('A', 101), "MPN", "Desc", null, null, null, null, null, null, null, null, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PartNumber);
    }

    [Fact]
    public void Validate_EmptyDescription_HasError()
    {
        var command = new CreatePartCommand(
            "PN", "MPN", "", null, null, null, null, null, null, null, null, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_EmptyManufacturerPartNumber_HasError()
    {
        var command = new CreatePartCommand(
            "PN", "", "Desc", null, null, null, null, null, null, null, null, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ManufacturerPartNumber);
    }
}
