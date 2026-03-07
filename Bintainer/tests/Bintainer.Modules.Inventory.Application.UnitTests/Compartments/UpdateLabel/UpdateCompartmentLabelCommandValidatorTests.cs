using Bintainer.Modules.Inventory.Application.Compartments.UpdateLabel;
using FluentValidation.TestHelper;

namespace Bintainer.Modules.Inventory.Application.UnitTests.Compartments.UpdateLabel;

public class UpdateCompartmentLabelCommandValidatorTests
{
    private readonly UpdateCompartmentLabelCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_NoErrors()
    {
        var command = new UpdateCompartmentLabelCommand(Guid.NewGuid(), "Label");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyCompartmentId_HasError()
    {
        var command = new UpdateCompartmentLabelCommand(Guid.Empty, "Label");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CompartmentId);
    }

    [Fact]
    public void Validate_EmptyLabel_HasError()
    {
        var command = new UpdateCompartmentLabelCommand(Guid.NewGuid(), "");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Label);
    }

    [Fact]
    public void Validate_LabelTooLong_HasError()
    {
        var command = new UpdateCompartmentLabelCommand(Guid.NewGuid(), new string('A', 101));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Label);
    }
}
