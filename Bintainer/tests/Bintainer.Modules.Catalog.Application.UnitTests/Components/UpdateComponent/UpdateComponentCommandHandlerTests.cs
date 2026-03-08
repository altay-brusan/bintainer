using Bintainer.Common.Application.ActivityLog;
using Bintainer.Common.Application.Authorization;
using Bintainer.Modules.Catalog.Application.Abstractions.Data;
using Bintainer.Modules.Catalog.Application.Components.UpdateComponent;
using Bintainer.Modules.Catalog.Domain.Components;

namespace Bintainer.Modules.Catalog.Application.UnitTests.Components.UpdateComponent;

public class UpdateComponentCommandHandlerTests
{
    private readonly IComponentRepository _componentRepository = Substitute.For<IComponentRepository>();
    private readonly IActivityLogger _activityLogger = Substitute.For<IActivityLogger>();
    private readonly ICurrentUserService _currentUserService = Substitute.For<ICurrentUserService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly UpdateComponentCommandHandler _handler;

    public UpdateComponentCommandHandlerTests()
    {
        _handler = new UpdateComponentCommandHandler(_componentRepository, _activityLogger, _currentUserService, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ComponentFound_ReturnsSuccess()
    {
        var componentId = Guid.NewGuid();
        var component = Component.Create("PN", "MPN", "Desc", null, null, null, null, null, null, null, null, null, null, null, 0);
        _componentRepository.GetByIdAsync(componentId, Arg.Any<CancellationToken>()).Returns(component);

        var command = new UpdateComponentCommand(
            componentId, "PN-2", "MPN-2", "Desc 2", null, null, null, null, null, null, null, null, null, null, null, 0);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ComponentNotFound_ReturnsFailure()
    {
        var componentId = Guid.NewGuid();
        _componentRepository.GetByIdAsync(componentId, Arg.Any<CancellationToken>()).Returns((Component?)null);

        var command = new UpdateComponentCommand(
            componentId, "PN", "MPN", "Desc", null, null, null, null, null, null, null, null, null, null, null, 0);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Components.NotFound");
    }

    [Fact]
    public async Task Handle_Success_CallsSaveChanges()
    {
        var componentId = Guid.NewGuid();
        var component = Component.Create("PN", "MPN", "Desc", null, null, null, null, null, null, null, null, null, null, null, 0);
        _componentRepository.GetByIdAsync(componentId, Arg.Any<CancellationToken>()).Returns(component);

        var command = new UpdateComponentCommand(
            componentId, "PN-2", "MPN-2", "Desc 2", null, null, null, null, null, null, null, null, null, null, null, 0);

        await _handler.Handle(command, CancellationToken.None);

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
