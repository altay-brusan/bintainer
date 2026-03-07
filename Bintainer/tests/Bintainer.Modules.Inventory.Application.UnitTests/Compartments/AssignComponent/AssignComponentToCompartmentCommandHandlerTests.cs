using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Application.Compartments.AssignComponent;
using Bintainer.Modules.Inventory.Domain.Compartments;
using Bintainer.Modules.Inventory.Domain.Components;
using Bintainer.Modules.Inventory.Domain.StorageUnits;

namespace Bintainer.Modules.Inventory.Application.UnitTests.Compartments.AssignComponent;

public class AssignComponentToCompartmentCommandHandlerTests
{
    private readonly ICompartmentRepository _compartmentRepository = Substitute.For<ICompartmentRepository>();
    private readonly IComponentRepository _componentRepository = Substitute.For<IComponentRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly AssignComponentToCompartmentCommandHandler _handler;

    public AssignComponentToCompartmentCommandHandlerTests()
    {
        _handler = new AssignComponentToCompartmentCommandHandler(
            _compartmentRepository, _componentRepository, _unitOfWork);
    }

    private static Compartment CreateCompartment()
    {
        var unit = StorageUnit.Create("Test", 1, 1, 1, Guid.NewGuid());
        return unit.Bins.First().Compartments.First();
    }

    [Fact]
    public async Task Handle_Valid_ReturnsSuccess()
    {
        var compartmentId = Guid.NewGuid();
        var componentId = Guid.NewGuid();
        var compartment = CreateCompartment();
        var component = Component.Create("PN", "MPN", "Desc", null, null, null, null, null, null, null, null, null, null, null, 0);

        _compartmentRepository.GetByIdAsync(compartmentId, Arg.Any<CancellationToken>()).Returns(compartment);
        _componentRepository.GetByIdAsync(componentId, Arg.Any<CancellationToken>()).Returns(component);

        var command = new AssignComponentToCompartmentCommand(compartmentId, componentId, 5);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        compartment.ComponentId.Should().Be(componentId);
        compartment.Quantity.Should().Be(5);
    }

    [Fact]
    public async Task Handle_CompartmentNotFound_ReturnsFailure()
    {
        var compartmentId = Guid.NewGuid();
        _compartmentRepository.GetByIdAsync(compartmentId, Arg.Any<CancellationToken>())
            .Returns((Compartment?)null);

        var command = new AssignComponentToCompartmentCommand(compartmentId, Guid.NewGuid(), 5);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Compartments.NotFound");
    }

    [Fact]
    public async Task Handle_ComponentNotFound_ReturnsFailure()
    {
        var compartmentId = Guid.NewGuid();
        var componentId = Guid.NewGuid();
        var compartment = CreateCompartment();
        _compartmentRepository.GetByIdAsync(compartmentId, Arg.Any<CancellationToken>()).Returns(compartment);
        _componentRepository.GetByIdAsync(componentId, Arg.Any<CancellationToken>()).Returns((Component?)null);

        var command = new AssignComponentToCompartmentCommand(compartmentId, componentId, 5);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Components.NotFound");
    }
}
