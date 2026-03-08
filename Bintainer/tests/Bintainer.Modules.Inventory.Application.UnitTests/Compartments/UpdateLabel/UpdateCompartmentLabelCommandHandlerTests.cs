using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Application.Compartments.UpdateLabel;
using Bintainer.Modules.Inventory.Domain.Compartments;
using Bintainer.Modules.Inventory.Domain.StorageUnits;

namespace Bintainer.Modules.Inventory.Application.UnitTests.Compartments.UpdateLabel;

public class UpdateCompartmentLabelCommandHandlerTests
{
    private readonly ICompartmentRepository _compartmentRepository = Substitute.For<ICompartmentRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly UpdateCompartmentLabelCommandHandler _handler;

    public UpdateCompartmentLabelCommandHandlerTests()
    {
        _handler = new UpdateCompartmentLabelCommandHandler(
            _compartmentRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_Valid_ReturnsSuccessAndUpdatesLabel()
    {
        var compartmentId = Guid.NewGuid();
        var unit = StorageUnit.Create("Test", 1, 1, 1, Guid.NewGuid());
        var compartment = unit.Bins.First().Compartments.First();
        _compartmentRepository.GetByIdAsync(compartmentId, Arg.Any<CancellationToken>()).Returns(compartment);

        var command = new UpdateCompartmentLabelCommand(compartmentId, "My Custom Label");
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        compartment.Label.Should().Be("My Custom Label");
    }

    [Fact]
    public async Task Handle_CompartmentNotFound_ReturnsFailure()
    {
        var compartmentId = Guid.NewGuid();
        _compartmentRepository.GetByIdAsync(compartmentId, Arg.Any<CancellationToken>())
            .Returns((Compartment?)null);

        var command = new UpdateCompartmentLabelCommand(compartmentId, "Label");
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Compartments.NotFound");
    }
}
