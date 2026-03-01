using Bintainer.Modules.Inventory.Application.Inventories.CreateInventory;
using FluentValidation.TestHelper;

namespace Bintainer.Modules.Inventory.Application.UnitTests.Inventories.CreateInventory;

public class CreateInventoryCommandValidatorTests
{
    private readonly CreateInventoryCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_NoErrors()
    {
        var command = new CreateInventoryCommand("My Inventory", Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyName_HasError()
    {
        var command = new CreateInventoryCommand("", Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_NameTooLong_HasError()
    {
        var command = new CreateInventoryCommand(new string('A', 201), Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_EmptyUserId_HasError()
    {
        var command = new CreateInventoryCommand("My Inventory", Guid.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }
}
