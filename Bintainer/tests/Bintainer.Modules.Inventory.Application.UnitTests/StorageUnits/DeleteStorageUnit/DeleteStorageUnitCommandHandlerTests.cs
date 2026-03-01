using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Application.StorageUnits.DeleteStorageUnit;
using Bintainer.Modules.Inventory.Domain.StorageUnits;

namespace Bintainer.Modules.Inventory.Application.UnitTests.StorageUnits.DeleteStorageUnit;

public class DeleteStorageUnitCommandHandlerTests
{
    private readonly IStorageUnitRepository _storageUnitRepository = Substitute.For<IStorageUnitRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly DeleteStorageUnitCommandHandler _handler;

    public DeleteStorageUnitCommandHandlerTests()
    {
        _handler = new DeleteStorageUnitCommandHandler(_storageUnitRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_StorageUnitFound_ReturnsSuccess()
    {
        var storageUnitId = Guid.NewGuid();
        var storageUnit = StorageUnit.Create("Test", 1, 1, 1, Guid.NewGuid());
        _storageUnitRepository.GetByIdAsync(storageUnitId, Arg.Any<CancellationToken>())
            .Returns(storageUnit);

        var command = new DeleteStorageUnitCommand(storageUnitId);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_StorageUnitFound_CallsRemove()
    {
        var storageUnitId = Guid.NewGuid();
        var storageUnit = StorageUnit.Create("Test", 1, 1, 1, Guid.NewGuid());
        _storageUnitRepository.GetByIdAsync(storageUnitId, Arg.Any<CancellationToken>())
            .Returns(storageUnit);

        var command = new DeleteStorageUnitCommand(storageUnitId);
        await _handler.Handle(command, CancellationToken.None);

        _storageUnitRepository.Received(1).Remove(storageUnit);
    }

    [Fact]
    public async Task Handle_StorageUnitFound_RaisesDomainEvent()
    {
        var storageUnitId = Guid.NewGuid();
        var storageUnit = StorageUnit.Create("Test", 1, 1, 1, Guid.NewGuid());
        _storageUnitRepository.GetByIdAsync(storageUnitId, Arg.Any<CancellationToken>())
            .Returns(storageUnit);

        var command = new DeleteStorageUnitCommand(storageUnitId);
        await _handler.Handle(command, CancellationToken.None);

        storageUnit.DomainEvents.Should().Contain(e => e is StorageUnitDeletedDomainEvent);
    }

    [Fact]
    public async Task Handle_StorageUnitNotFound_ReturnsFailure()
    {
        var storageUnitId = Guid.NewGuid();
        _storageUnitRepository.GetByIdAsync(storageUnitId, Arg.Any<CancellationToken>())
            .Returns((StorageUnit?)null);

        var command = new DeleteStorageUnitCommand(storageUnitId);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("StorageUnits.NotFound");
    }
}
