using System.Text.Json;
using Bintainer.Common.Application.Authorization;
using Bintainer.Common.Application.EventBus;
using Bintainer.Common.Application.Messaging;
using Bintainer.Modules.Catalog.Domain.BomImports;
using Bintainer.Modules.Catalog.Domain.Categories;
using Bintainer.Modules.Catalog.Domain.Components;
using Bintainer.Modules.Catalog.Domain.Footprints;

namespace Bintainer.Modules.Catalog.Application.ActivityLogging;

internal sealed class ComponentCreatedActivityHandler(IEventBus eventBus, ICurrentUserService currentUserService)
    : IDomainEventHandler<ComponentCreatedDomainEvent>
{
    public Task Handle(ComponentCreatedDomainEvent notification, CancellationToken cancellationToken) =>
        eventBus.PublishAsync(new ActivityLoggedIntegrationEvent(
            notification.Id, notification.OccurredOnUtc,
            currentUserService.UserId, "ComponentCreated", "Component",
            notification.ComponentId, notification.PartNumber,
            $"{notification.PartNumber} added", null), cancellationToken);
}

internal sealed class ComponentUpdatedActivityHandler(IEventBus eventBus, ICurrentUserService currentUserService)
    : IDomainEventHandler<ComponentUpdatedDomainEvent>
{
    public Task Handle(ComponentUpdatedDomainEvent notification, CancellationToken cancellationToken) =>
        eventBus.PublishAsync(new ActivityLoggedIntegrationEvent(
            notification.Id, notification.OccurredOnUtc,
            currentUserService.UserId, "ComponentUpdated", "Component",
            notification.ComponentId, notification.PartNumber,
            $"{notification.PartNumber} updated", null), cancellationToken);
}

internal sealed class ComponentDeletedActivityHandler(IEventBus eventBus, ICurrentUserService currentUserService)
    : IDomainEventHandler<ComponentDeletedDomainEvent>
{
    public Task Handle(ComponentDeletedDomainEvent notification, CancellationToken cancellationToken) =>
        eventBus.PublishAsync(new ActivityLoggedIntegrationEvent(
            notification.Id, notification.OccurredOnUtc,
            currentUserService.UserId, "ComponentDeleted", "Component",
            notification.ComponentId, notification.PartNumber,
            $"{notification.PartNumber} deleted", null), cancellationToken);
}

internal sealed class ComponentImageUploadedActivityHandler(IEventBus eventBus, ICurrentUserService currentUserService)
    : IDomainEventHandler<ComponentImageUploadedDomainEvent>
{
    public Task Handle(ComponentImageUploadedDomainEvent notification, CancellationToken cancellationToken) =>
        eventBus.PublishAsync(new ActivityLoggedIntegrationEvent(
            notification.Id, notification.OccurredOnUtc,
            currentUserService.UserId, "ComponentImageUploaded", "Component",
            notification.ComponentId, null,
            "Component image uploaded", null), cancellationToken);
}

internal sealed class CategoryCreatedActivityHandler(IEventBus eventBus, ICurrentUserService currentUserService)
    : IDomainEventHandler<CategoryCreatedDomainEvent>
{
    public Task Handle(CategoryCreatedDomainEvent notification, CancellationToken cancellationToken) =>
        eventBus.PublishAsync(new ActivityLoggedIntegrationEvent(
            notification.Id, notification.OccurredOnUtc,
            currentUserService.UserId, "CategoryCreated", "Category",
            notification.CategoryId, notification.Name,
            $"Category {notification.Name} created", null), cancellationToken);
}

internal sealed class CategoryUpdatedActivityHandler(IEventBus eventBus, ICurrentUserService currentUserService)
    : IDomainEventHandler<CategoryUpdatedDomainEvent>
{
    public Task Handle(CategoryUpdatedDomainEvent notification, CancellationToken cancellationToken) =>
        eventBus.PublishAsync(new ActivityLoggedIntegrationEvent(
            notification.Id, notification.OccurredOnUtc,
            currentUserService.UserId, "CategoryUpdated", "Category",
            notification.CategoryId, notification.Name,
            $"Category {notification.Name} updated", null), cancellationToken);
}

internal sealed class CategoryDeletedActivityHandler(IEventBus eventBus, ICurrentUserService currentUserService)
    : IDomainEventHandler<CategoryDeletedDomainEvent>
{
    public Task Handle(CategoryDeletedDomainEvent notification, CancellationToken cancellationToken) =>
        eventBus.PublishAsync(new ActivityLoggedIntegrationEvent(
            notification.Id, notification.OccurredOnUtc,
            currentUserService.UserId, "CategoryDeleted", "Category",
            notification.CategoryId, notification.Name,
            $"Category {notification.Name} deleted", null), cancellationToken);
}

internal sealed class FootprintCreatedActivityHandler(IEventBus eventBus, ICurrentUserService currentUserService)
    : IDomainEventHandler<FootprintCreatedDomainEvent>
{
    public Task Handle(FootprintCreatedDomainEvent notification, CancellationToken cancellationToken) =>
        eventBus.PublishAsync(new ActivityLoggedIntegrationEvent(
            notification.Id, notification.OccurredOnUtc,
            currentUserService.UserId, "FootprintCreated", "Footprint",
            notification.FootprintId, notification.Name,
            $"Footprint {notification.Name} created", null), cancellationToken);
}

internal sealed class FootprintUpdatedActivityHandler(IEventBus eventBus, ICurrentUserService currentUserService)
    : IDomainEventHandler<FootprintUpdatedDomainEvent>
{
    public Task Handle(FootprintUpdatedDomainEvent notification, CancellationToken cancellationToken) =>
        eventBus.PublishAsync(new ActivityLoggedIntegrationEvent(
            notification.Id, notification.OccurredOnUtc,
            currentUserService.UserId, "FootprintUpdated", "Footprint",
            notification.FootprintId, notification.Name,
            $"Footprint {notification.Name} updated", null), cancellationToken);
}

internal sealed class FootprintDeletedActivityHandler(IEventBus eventBus, ICurrentUserService currentUserService)
    : IDomainEventHandler<FootprintDeletedDomainEvent>
{
    public Task Handle(FootprintDeletedDomainEvent notification, CancellationToken cancellationToken) =>
        eventBus.PublishAsync(new ActivityLoggedIntegrationEvent(
            notification.Id, notification.OccurredOnUtc,
            currentUserService.UserId, "FootprintDeleted", "Footprint",
            notification.FootprintId, notification.Name,
            $"Footprint {notification.Name} deleted", null), cancellationToken);
}

internal sealed class BomImportedActivityHandler(IEventBus eventBus, ICurrentUserService currentUserService)
    : IDomainEventHandler<BomImportedDomainEvent>
{
    public Task Handle(BomImportedDomainEvent notification, CancellationToken cancellationToken) =>
        eventBus.PublishAsync(new ActivityLoggedIntegrationEvent(
            notification.Id, notification.OccurredOnUtc,
            currentUserService.UserId, "BomImported", "BomImport",
            notification.BomImportId, notification.FileName,
            $"BOM {notification.FileName} imported",
            JsonSerializer.Serialize(new { notification.LineCount })), cancellationToken);
}
