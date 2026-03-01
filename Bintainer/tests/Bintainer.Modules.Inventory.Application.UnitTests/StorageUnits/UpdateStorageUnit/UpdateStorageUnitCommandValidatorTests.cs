using Bintainer.Modules.Inventory.Application.StorageUnits.UpdateStorageUnit;
using FluentValidation.TestHelper;

namespace Bintainer.Modules.Inventory.Application.UnitTests.StorageUnits.UpdateStorageUnit;

public class UpdateStorageUnitCommandValidatorTests
{
    private readonly UpdateStorageUnitCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_NoErrors()
    {
        var command = new UpdateStorageUnitCommand(Guid.NewGuid(), "New Name");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyId_HasError()
    {
        var command = new UpdateStorageUnitCommand(Guid.Empty, "New Name");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StorageUnitId);
    }

    [Fact]
    public void Validate_EmptyName_HasError()
    {
        var command = new UpdateStorageUnitCommand(Guid.NewGuid(), "");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_NameTooLong_HasError()
    {
        var command = new UpdateStorageUnitCommand(Guid.NewGuid(), new string('A', 201));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}
