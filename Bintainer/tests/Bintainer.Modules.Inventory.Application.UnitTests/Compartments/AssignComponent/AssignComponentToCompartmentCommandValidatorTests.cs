using Bintainer.Modules.Inventory.Application.Compartments.AssignComponent;
using FluentValidation.TestHelper;

namespace Bintainer.Modules.Inventory.Application.UnitTests.Compartments.AssignComponent;

public class AssignComponentToCompartmentCommandValidatorTests
{
    private readonly AssignComponentToCompartmentCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_NoErrors()
    {
        var command = new AssignComponentToCompartmentCommand(Guid.NewGuid(), Guid.NewGuid(), 5);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyCompartmentId_HasError()
    {
        var command = new AssignComponentToCompartmentCommand(Guid.Empty, Guid.NewGuid(), 5);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CompartmentId);
    }

    [Fact]
    public void Validate_EmptyComponentId_HasError()
    {
        var command = new AssignComponentToCompartmentCommand(Guid.NewGuid(), Guid.Empty, 5);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ComponentId);
    }

    [Fact]
    public void Validate_ZeroQuantity_HasError()
    {
        var command = new AssignComponentToCompartmentCommand(Guid.NewGuid(), Guid.NewGuid(), 0);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }
}
