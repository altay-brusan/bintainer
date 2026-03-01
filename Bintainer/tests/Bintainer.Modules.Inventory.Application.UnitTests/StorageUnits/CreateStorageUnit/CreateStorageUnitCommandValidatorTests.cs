using Bintainer.Modules.Inventory.Application.StorageUnits.CreateStorageUnit;
using FluentValidation.TestHelper;

namespace Bintainer.Modules.Inventory.Application.UnitTests.StorageUnits.CreateStorageUnit;

public class CreateStorageUnitCommandValidatorTests
{
    private readonly CreateStorageUnitCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_NoErrors()
    {
        var command = new CreateStorageUnitCommand("Shelf A", 3, 2, 4, Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyName_HasError()
    {
        var command = new CreateStorageUnitCommand("", 3, 2, 4, Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_NameTooLong_HasError()
    {
        var command = new CreateStorageUnitCommand(new string('A', 201), 3, 2, 4, Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ColumnsZero_HasError()
    {
        var command = new CreateStorageUnitCommand("Shelf A", 0, 2, 4, Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Columns);
    }

    [Fact]
    public void Validate_RowsZero_HasError()
    {
        var command = new CreateStorageUnitCommand("Shelf A", 3, 0, 4, Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Rows);
    }

    [Fact]
    public void Validate_CompartmentCountZero_HasError()
    {
        var command = new CreateStorageUnitCommand("Shelf A", 3, 2, 0, Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CompartmentCount);
    }

    [Fact]
    public void Validate_EmptyInventoryId_HasError()
    {
        var command = new CreateStorageUnitCommand("Shelf A", 3, 2, 4, Guid.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.InventoryId);
    }
}
