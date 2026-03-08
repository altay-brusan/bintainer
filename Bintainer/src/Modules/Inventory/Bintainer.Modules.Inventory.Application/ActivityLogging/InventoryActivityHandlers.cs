using System.Text.Json;
using Bintainer.Common.Application.Authorization;
using Bintainer.Common.Application.EventBus;
using Bintainer.Common.Application.Messaging;
using Bintainer.Modules.Inventory.Domain.Bins;
using Bintainer.Modules.Inventory.Domain.Compartments;
using Bintainer.Modules.Inventory.Domain.Inventories;
using Bintainer.Modules.Inventory.Domain.Movements;
using Bintainer.Modules.Inventory.Domain.StorageUnits;

namespace Bintainer.Modules.Inventory.Application.ActivityLogging;

internal sealed class InventoryCreatedActivityHandler(IEventBus eventBus, ICurrentUserService currentUserService)
    : IDomainEventHandler<InventoryCreatedDomainEvent>
{
    public Task Handle(InventoryCreatedDomainEvent notification, CancellationToken cancellationToken) =>
        eventBus.PublishAsync(new ActivityLoggedIntegrationEvent(
            notification.Id, notification.OccurredOnUtc,
            currentUserService.UserId, "InventoryCreated", "Inventory",
            notification.InventoryId, notification.Name,
            $"{notification.Name} created", null), cancellationToken);
}

internal sealed class StorageUnitCreatedActivityHandler(IEventBus eventBus, ICurrentUserService currentUserService)
    : IDomainEventHandler<StorageUnitCreatedDomainEvent>
{
    public Task Handle(StorageUnitCreatedDomainEvent notification, CancellationToken cancellationToken) =>
        eventBus.PublishAsync(new ActivityLoggedIntegrationEvent(
            notification.Id, notification.OccurredOnUtc,
            currentUserService.UserId, "StorageUnitCreated", "StorageUnit",
            notification.StorageUnitId, notification.Name,
            $"Storage unit {notification.Name} created", null), cancellationToken);
}

internal sealed class StorageUnitUpdatedActivityHandler(IEventBus eventBus, ICurrentUserService currentUserService)
    : IDomainEventHandler<StorageUnitUpdatedDomainEvent>
{
    public Task Handle(StorageUnitUpdatedDomainEvent notification, CancellationToken cancellationToken) =>
        eventBus.PublishAsync(new ActivityLoggedIntegrationEvent(
            notification.Id, notification.OccurredOnUtc,
            currentUserService.UserId, "StorageUnitUpdated", "StorageUnit",
            notification.StorageUnitId, notification.Name,
            $"Storage unit {notification.Name} updated", null), cancellationToken);
}

internal sealed class StorageUnitDeletedActivityHandler(IEventBus eventBus, ICurrentUserService currentUserService)
    : IDomainEventHandler<StorageUnitDeletedDomainEvent>
{
    public Task Handle(StorageUnitDeletedDomainEvent notification, CancellationToken cancellationToken) =>
        eventBus.PublishAsync(new ActivityLoggedIntegrationEvent(
            notification.Id, notification.OccurredOnUtc,
            currentUserService.UserId, "StorageUnitDeleted", "StorageUnit",
            notification.StorageUnitId, notification.Name,
            $"Storage unit {notification.Name} deleted", null), cancellationToken);
}

internal sealed class BinActivatedActivityHandler(IEventBus eventBus, ICurrentUserService currentUserService)
    : IDomainEventHandler<BinActivatedDomainEvent>
{
    public Task Handle(BinActivatedDomainEvent notification, CancellationToken cancellationToken) =>
        eventBus.PublishAsync(new ActivityLoggedIntegrationEvent(
            notification.Id, notification.OccurredOnUtc,
            currentUserService.UserId, "BinActivated", "Bin",
            notification.BinId, null,
            "Bin activated", null), cancellationToken);
}

internal sealed class BinDeactivatedActivityHandler(IEventBus eventBus, ICurrentUserService currentUserService)
    : IDomainEventHandler<BinDeactivatedDomainEvent>
{
    public Task Handle(BinDeactivatedDomainEvent notification, CancellationToken cancellationToken) =>
        eventBus.PublishAsync(new ActivityLoggedIntegrationEvent(
            notification.Id, notification.OccurredOnUtc,
            currentUserService.UserId, "BinDeactivated", "Bin",
            notification.BinId, null,
            "Bin deactivated", null), cancellationToken);
}

internal sealed class CompartmentComponentAssignedActivityHandler(IEventBus eventBus, ICurrentUserService currentUserService)
    : IDomainEventHandler<CompartmentComponentAssignedDomainEvent>
{
    public Task Handle(CompartmentComponentAssignedDomainEvent notification, CancellationToken cancellationToken) =>
        eventBus.PublishAsync(new ActivityLoggedIntegrationEvent(
            notification.Id, notification.OccurredOnUtc,
            currentUserService.UserId, "ComponentAssigned", "Compartment",
            notification.CompartmentId, null,
            "Component assigned to compartment",
            JsonSerializer.Serialize(new { notification.ComponentId, notification.Quantity })), cancellationToken);
}

internal sealed class CompartmentComponentRemovedActivityHandler(IEventBus eventBus, ICurrentUserService currentUserService)
    : IDomainEventHandler<CompartmentComponentRemovedDomainEvent>
{
    public Task Handle(CompartmentComponentRemovedDomainEvent notification, CancellationToken cancellationToken) =>
        eventBus.PublishAsync(new ActivityLoggedIntegrationEvent(
            notification.Id, notification.OccurredOnUtc,
            currentUserService.UserId, "ComponentRemoved", "Compartment",
            notification.CompartmentId, null,
            "Component removed from compartment", null), cancellationToken);
}

internal sealed class CompartmentLabelUpdatedActivityHandler(IEventBus eventBus, ICurrentUserService currentUserService)
    : IDomainEventHandler<CompartmentLabelUpdatedDomainEvent>
{
    public Task Handle(CompartmentLabelUpdatedDomainEvent notification, CancellationToken cancellationToken) =>
        eventBus.PublishAsync(new ActivityLoggedIntegrationEvent(
            notification.Id, notification.OccurredOnUtc,
            currentUserService.UserId, "CompartmentLabelUpdated", "Compartment",
            notification.CompartmentId, notification.Label,
            $"Compartment label updated to {notification.Label}", null), cancellationToken);
}

internal sealed class CompartmentActivatedActivityHandler(IEventBus eventBus, ICurrentUserService currentUserService)
    : IDomainEventHandler<CompartmentActivatedDomainEvent>
{
    public Task Handle(CompartmentActivatedDomainEvent notification, CancellationToken cancellationToken) =>
        eventBus.PublishAsync(new ActivityLoggedIntegrationEvent(
            notification.Id, notification.OccurredOnUtc,
            currentUserService.UserId, "CompartmentActivated", "Compartment",
            notification.CompartmentId, null,
            "Compartment activated", null), cancellationToken);
}

internal sealed class CompartmentDeactivatedActivityHandler(IEventBus eventBus, ICurrentUserService currentUserService)
    : IDomainEventHandler<CompartmentDeactivatedDomainEvent>
{
    public Task Handle(CompartmentDeactivatedDomainEvent notification, CancellationToken cancellationToken) =>
        eventBus.PublishAsync(new ActivityLoggedIntegrationEvent(
            notification.Id, notification.OccurredOnUtc,
            currentUserService.UserId, "CompartmentDeactivated", "Compartment",
            notification.CompartmentId, null,
            "Compartment deactivated", null), cancellationToken);
}

internal sealed class ComponentQuantityAdjustedActivityHandler(IEventBus eventBus, ICurrentUserService currentUserService)
    : IDomainEventHandler<ComponentQuantityAdjustedDomainEvent>
{
    public Task Handle(ComponentQuantityAdjustedDomainEvent notification, CancellationToken cancellationToken) =>
        eventBus.PublishAsync(new ActivityLoggedIntegrationEvent(
            notification.Id, notification.OccurredOnUtc,
            currentUserService.UserId, "QuantityAdjusted", "Component",
            notification.ComponentId, null,
            $"Component quantity {notification.Action.ToLowerInvariant()}",
            JsonSerializer.Serialize(new { notification.CompartmentId, notification.Action, notification.Quantity })), cancellationToken);
}

internal sealed class ComponentMovedActivityHandler(IEventBus eventBus, ICurrentUserService currentUserService)
    : IDomainEventHandler<ComponentMovedDomainEvent>
{
    public Task Handle(ComponentMovedDomainEvent notification, CancellationToken cancellationToken) =>
        eventBus.PublishAsync(new ActivityLoggedIntegrationEvent(
            notification.Id, notification.OccurredOnUtc,
            currentUserService.UserId, "ComponentMoved", "Component",
            notification.ComponentId, null,
            "Component moved",
            JsonSerializer.Serialize(new { notification.SourceCompartmentId, notification.DestinationCompartmentId, notification.Quantity })), cancellationToken);
}
