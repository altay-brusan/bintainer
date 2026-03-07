using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Application.Components.DeleteComponent;
using Bintainer.Modules.Inventory.Domain.Components;

namespace Bintainer.Modules.Inventory.Application.UnitTests.Components.DeleteComponent;

public class DeleteComponentCommandHandlerTests
{
    private readonly IComponentRepository _componentRepository = Substitute.For<IComponentRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly DeleteComponentCommandHandler _handler;

    public DeleteComponentCommandHandlerTests()
    {
        _handler = new DeleteComponentCommandHandler(_componentRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ComponentFound_ReturnsSuccess()
    {
        var componentId = Guid.NewGuid();
        var component = Component.Create("PN", "MPN", "Desc", null, null, null, null, null, null, null, null, null, null, null, 0);
        _componentRepository.GetByIdAsync(componentId, Arg.Any<CancellationToken>()).Returns(component);

        var command = new DeleteComponentCommand(componentId);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ComponentFound_CallsRemove()
    {
        var componentId = Guid.NewGuid();
        var component = Component.Create("PN", "MPN", "Desc", null, null, null, null, null, null, null, null, null, null, null, 0);
        _componentRepository.GetByIdAsync(componentId, Arg.Any<CancellationToken>()).Returns(component);

        var command = new DeleteComponentCommand(componentId);
        await _handler.Handle(command, CancellationToken.None);

        _componentRepository.Received(1).Remove(component);
    }

    [Fact]
    public async Task Handle_ComponentFound_RaisesDomainEvent()
    {
        var componentId = Guid.NewGuid();
        var component = Component.Create("PN", "MPN", "Desc", null, null, null, null, null, null, null, null, null, null, null, 0);
        _componentRepository.GetByIdAsync(componentId, Arg.Any<CancellationToken>()).Returns(component);

        var command = new DeleteComponentCommand(componentId);
        await _handler.Handle(command, CancellationToken.None);

        component.DomainEvents.Should().Contain(e => e is ComponentDeletedDomainEvent);
    }

    [Fact]
    public async Task Handle_ComponentNotFound_ReturnsFailure()
    {
        var componentId = Guid.NewGuid();
        _componentRepository.GetByIdAsync(componentId, Arg.Any<CancellationToken>()).Returns((Component?)null);

        var command = new DeleteComponentCommand(componentId);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Components.NotFound");
    }
}
