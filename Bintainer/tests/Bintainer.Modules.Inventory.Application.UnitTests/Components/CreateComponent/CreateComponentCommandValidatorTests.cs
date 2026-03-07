using Bintainer.Modules.Inventory.Application.Components.CreateComponent;
using FluentValidation.TestHelper;

namespace Bintainer.Modules.Inventory.Application.UnitTests.Components.CreateComponent;

public class CreateComponentCommandValidatorTests
{
    private readonly CreateComponentCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_NoErrors()
    {
        var command = new CreateComponentCommand(
            "PN-001", "MPN-001", "Resistor", null, null, null, null, null, null, null, null, null, null, null, 0);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyPartNumber_HasError()
    {
        var command = new CreateComponentCommand(
            "", "MPN", "Desc", null, null, null, null, null, null, null, null, null, null, null, 0);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PartNumber);
    }

    [Fact]
    public void Validate_PartNumberTooLong_HasError()
    {
        var command = new CreateComponentCommand(
            new string('A', 101), "MPN", "Desc", null, null, null, null, null, null, null, null, null, null, null, 0);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PartNumber);
    }

    [Fact]
    public void Validate_EmptyDescription_HasError()
    {
        var command = new CreateComponentCommand(
            "PN", "MPN", "", null, null, null, null, null, null, null, null, null, null, null, 0);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_EmptyManufacturerPartNumber_HasError()
    {
        var command = new CreateComponentCommand(
            "PN", "", "Desc", null, null, null, null, null, null, null, null, null, null, null, 0);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ManufacturerPartNumber);
    }
}
