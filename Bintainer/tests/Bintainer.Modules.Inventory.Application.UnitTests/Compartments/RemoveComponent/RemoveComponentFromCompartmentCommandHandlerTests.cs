using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Application.Compartments.RemoveComponent;
using Bintainer.Modules.Inventory.Domain.Compartments;
using Bintainer.Modules.Inventory.Domain.StorageUnits;

namespace Bintainer.Modules.Inventory.Application.UnitTests.Compartments.RemoveComponent;

public class RemoveComponentFromCompartmentCommandHandlerTests
{
    private readonly ICompartmentRepository _compartmentRepository = Substitute.For<ICompartmentRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly RemoveComponentFromCompartmentCommandHandler _handler;

    public RemoveComponentFromCompartmentCommandHandlerTests()
    {
        _handler = new RemoveComponentFromCompartmentCommandHandler(_compartmentRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_Valid_ReturnsSuccessAndClearsComponent()
    {
        var compartmentId = Guid.NewGuid();
        var unit = StorageUnit.Create("Test", 1, 1, 1, Guid.NewGuid());
        var compartment = unit.Bins.First().Compartments.First();
        compartment.AssignComponent(Guid.NewGuid(), 10);
        _compartmentRepository.GetByIdAsync(compartmentId, Arg.Any<CancellationToken>()).Returns(compartment);

        var command = new RemoveComponentFromCompartmentCommand(compartmentId);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        compartment.ComponentId.Should().BeNull();
        compartment.Quantity.Should().Be(0);
    }

    [Fact]
    public async Task Handle_CompartmentNotFound_ReturnsFailure()
    {
        var compartmentId = Guid.NewGuid();
        _compartmentRepository.GetByIdAsync(compartmentId, Arg.Any<CancellationToken>())
            .Returns((Compartment?)null);

        var command = new RemoveComponentFromCompartmentCommand(compartmentId);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Compartments.NotFound");
    }
}
