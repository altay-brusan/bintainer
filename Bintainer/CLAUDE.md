# Bintainer - Claude Code Guide

## Build / Run / Test

```bash
# Restore and build
dotnet build Bintainer.slnx

# Run the API
dotnet run --project src/API/Bintainer.Api

# Add EF Core migration (from repo root)
dotnet ef migrations add <MigrationName> --project src/Modules/Users/Bintainer.Modules.Users.Infrastructure --startup-project src/API/Bintainer.Api
dotnet ef migrations add <MigrationName> --project src/Modules/Inventory/Bintainer.Modules.Inventory.Infrastructure --startup-project src/API/Bintainer.Api
dotnet ef migrations add <MigrationName> --project src/Modules/ActivityLog/Bintainer.Modules.ActivityLog.Infrastructure --startup-project src/API/Bintainer.Api

# Apply migrations
dotnet ef database update --project src/Modules/Users/Bintainer.Modules.Users.Infrastructure --startup-project src/API/Bintainer.Api
dotnet ef database update --project src/Modules/Inventory/Bintainer.Modules.Inventory.Infrastructure --startup-project src/API/Bintainer.Api
dotnet ef database update --project src/Modules/ActivityLog/Bintainer.Modules.ActivityLog.Infrastructure --startup-project src/API/Bintainer.Api
```

## Solution Structure

```
Bintainer.slnx (.NET 9 — Clean Architecture modular monolith)
├── src/Common/
│   ├── Bintainer.Common.Domain          — Entity base, Result/Error, domain events
│   ├── Bintainer.Common.Application     — CQRS messaging, pipeline behaviors, abstractions
│   ├── Bintainer.Common.Infrastructure  — PostgreSQL, MassTransit, caching, interceptors
│   └── Bintainer.Common.Presentation   — Minimal API endpoints, ProblemDetails mapping
├── src/Modules/Users/
│   ├── Bintainer.Modules.Users.Domain          — User, RefreshToken entities
│   ├── Bintainer.Modules.Users.Application     — Register, Login, Refresh, Logout, GetCurrentUser
│   ├── Bintainer.Modules.Users.Presentation    — Auth endpoints (api/auth/*)
│   └── Bintainer.Modules.Users.Infrastructure  — UsersDbContext, Identity, JWT
├── src/Modules/Inventory/
│   ├── Bintainer.Modules.Inventory.Domain          — Inventory, StorageUnit, Bin, Compartment, Movement entities
│   ├── Bintainer.Modules.Inventory.Application     — CRUD for inventories, storage units, compartments; component qty/move; movements
│   ├── Bintainer.Modules.Inventory.Presentation    — Endpoints (api/inventories/*, api/storage-units/*, api/compartments/*, api/components/*/quantity|move, api/movements)
│   └── Bintainer.Modules.Inventory.Infrastructure  — InventoryDbContext, EF configs, repositories
├── src/Modules/Catalog/
│   ├── Bintainer.Modules.Catalog.Domain            — Component, Category, Footprint, BomImport entities
│   ├── Bintainer.Modules.Catalog.Application       — Component CRUD + search + image upload; categories; footprints; tags; BOM import
│   ├── Bintainer.Modules.Catalog.Presentation      — Endpoints (api/components/*, api/categories/*, api/footprints/*, api/tags, api/bom/*)
│   └── Bintainer.Modules.Catalog.Infrastructure    — CatalogDbContext, file storage, repositories
├── src/Modules/Reports/
│   ├── Bintainer.Modules.Reports.Application       — Report queries (summary, top components, low stock, utilization, timeline, distributions)
│   ├── Bintainer.Modules.Reports.Presentation      — Endpoints (api/reports/*)
│   └── Bintainer.Modules.Reports.Infrastructure    — ReportReadService (Dapper, cross-schema queries)
├── src/Modules/ActivityLog/
│   ├── Bintainer.Modules.ActivityLog.Domain         — ActivityEntry entity
│   ├── Bintainer.Modules.ActivityLog.Application    — GetActivityLog query, IActivityLogReadService
│   ├── Bintainer.Modules.ActivityLog.Presentation   — Endpoint (api/activity-log)
│   └── Bintainer.Modules.ActivityLog.Infrastructure — ActivityLogDbContext, ActivityLogger, read service
└── src/API/
    └── Bintainer.Api — Composition root (Serilog, Swagger, JWT auth, module wiring)
```

**Dependency flow:** API → Module.Infrastructure → Module.Application + Module.Presentation → Common layers → Common.Domain (leaf)

## Architecture & Patterns

- **Minimal API** endpoints via `IEndpoint` auto-discovery pattern
- **CQRS** with MediatR: `ICommand`/`IQuery` with `ICommandHandler`/`IQueryHandler`
- **Result pattern**: `Result<T>` with `IsSuccess`, `Error` (no exceptions for business logic)
- **Pipeline behaviors**: Validation (FluentValidation), Exception handling, Request logging
- **Repository pattern**: interfaces in Domain, implementations in Infrastructure
- **Unit of Work**: each module's DbContext implements `IUnitOfWork`
- **Domain events**: raised via `Entity.Raise()`, published by `PublishDomainEventsInterceptor`
- **Activity logging via domain events + MassTransit**: entities raise domain events → handlers publish `ActivityLoggedIntegrationEvent` via `IEventBus` → MassTransit delivers to `ActivityLoggedIntegrationEventConsumer` in ActivityLog module → saved to `activity` schema
- **Schema-per-module**: `users`, `inventory`, `activity` PostgreSQL schemas

## Domain Model

```
Inventory → StorageUnit → Bin (col, row) → Compartment (index, label, componentId?, quantity)
Component (catalog entry: partNumber, description, manufacturer, unitPrice, tags csv, attributes jsonb, lowStockThreshold, categoryId?, footprintId?)
Category (name, parentId?)
Footprint (name)
Movement (componentId, action=moved, quantity, date, userId, notes — physical moves only)
ActivityEntry (userId, action, entityType, entityId, entityName, message, details jsonb, timestamp)
BomImport (fileName, lines, matched/new counts)
```

A Compartment references a Component via `ComponentId`. A Component can exist in multiple Compartments.

## Database

- **PostgreSQL** — database name **`BintainerDb`**, using `Npgsql.EntityFrameworkCore.PostgreSQL`
- Connection string in `src/API/Bintainer.Api/appsettings.json` under `ConnectionStrings:DefaultConnection`
- **Snake_case naming** via `EFCore.NamingConventions`
- **Three DbContexts**:
  - `UsersDbContext` (extends `IdentityDbContext<IdentityUser>`) — schema `users`
  - `InventoryDbContext` (extends `DbContext`) — schema `inventory`
  - `ActivityLogDbContext` (extends `DbContext`) — schema `activity`
- JWT config in `modules.users.json`, inventory config in `modules.inventory.json`

## DI Registration

All services registered as **scoped**. Module registration via extension methods:
- `AddApplication(assemblies)` — MediatR + behaviors + validators
- `AddInfrastructure(connectionString)` — Npgsql, caching, MassTransit, clock, auth
- `AddUsersModule(config)` — Identity, JWT, user repos
- `AddInventoryModule(config)` — Inventory DbContext, repos
- `AddCatalogModule(config)` — Catalog DbContext, component repos, file storage
- `AddActivityLogModule(config)` — ActivityLog DbContext, ActivityLogger, read service
- `AddReportsModule(config)` — ReportReadService (no DbContext, uses Dapper with IDbConnectionFactory)

## Activity Logging

Activity logging is driven by **domain events**, not manual calls in command handlers.

### Flow

```
Entity method (e.g. Component.Create()) → raises domain event
    → SaveChangesAsync() → PublishDomainEventsInterceptor publishes via MediatR
    → IDomainEventHandler (activity handler) in the module's Application layer
    → publishes ActivityLoggedIntegrationEvent via IEventBus (MassTransit)
    → ActivityLoggedIntegrationEventConsumer (ActivityLog.Infrastructure)
    → IActivityLogger saves ActivityEntry to activity.activities table
```

### Activity Handlers (Domain Event → Integration Event)

Each module has an `ActivityLogging/` folder in its Application layer:
- `Inventory.Application/ActivityLogging/InventoryActivityHandlers.cs` — 13 handlers
- `Catalog.Application/ActivityLogging/CatalogActivityHandlers.cs` — 11 handlers

Handlers inject `IEventBus` and `ICurrentUserService`. They convert domain events into a shared `ActivityLoggedIntegrationEvent` (Common.Application) and publish via MassTransit.

### Consumer (Integration Event → Database)

`ActivityLog.Infrastructure/Consumers/ActivityLoggedIntegrationEventConsumer.cs` — single MassTransit consumer that receives all activity events and persists them via `IActivityLogger`. No other module calls `IActivityLogger` directly.

### ActivityEntry Schema (`activity.activities`)

| Column | Type | Description |
|---|---|---|
| `id` | uuid | Primary key |
| `user_id` | uuid | Who performed the action |
| `action` | text | Programmatic action (e.g. `ComponentCreated`, `QuantityAdjusted`) |
| `entity_type` | text | Entity type (e.g. `Component`, `StorageUnit`, `Compartment`) |
| `entity_id` | uuid | ID of the affected entity |
| `entity_name` | text? | Display name (e.g. part number, storage unit name) |
| `message` | text? | Human-readable message for UI (e.g. "STM32F103 added") |
| `details` | jsonb? | Extra context (e.g. `{"compartmentId":"...","quantity":10}`) |
| `timestamp` | timestamp | When the action occurred (UTC) |

### Tracked Actions

| Action | EntityType | Has Name | Has Details |
|---|---|---|---|
| `InventoryCreated` | Inventory | yes | — |
| `StorageUnitCreated/Updated/Deleted` | StorageUnit | yes | — |
| `BinActivated/Deactivated` | Bin | — | — |
| `ComponentAssigned` | Compartment | — | componentId, quantity |
| `ComponentRemoved` | Compartment | — | — |
| `CompartmentLabelUpdated` | Compartment | yes (label) | — |
| `CompartmentActivated/Deactivated` | Compartment | — | — |
| `QuantityAdjusted` | Component | — | compartmentId, action, quantity |
| `ComponentMoved` | Component | — | sourceId, destId, quantity |
| `ComponentCreated/Updated/Deleted` | Component | yes (partNumber) | — |
| `ComponentImageUploaded` | Component | — | — |
| `CategoryCreated/Updated/Deleted` | Category | yes | — |
| `FootprintCreated/Updated/Deleted` | Footprint | yes | — |
| `BomImported` | BomImport | yes (fileName) | lineCount |

### Adding Activity Logging to a New Action

1. Create a domain event in the module's Domain layer
2. Raise it from the entity method or command handler
3. Add an `IDomainEventHandler<T>` in the module's `ActivityLogging/` folder
4. The handler publishes `ActivityLoggedIntegrationEvent` via `IEventBus` with a human-readable message
5. The `ActivityLoggedIntegrationEventConsumer` in ActivityLog module handles persistence automatically

### API Endpoint

`GET /api/activity-log?action=&entityType=&entityId=&page=1&pageSize=20` — returns paged results with `userName` joined from users table.

## External Services

- **Serilog** for structured logging
- **MassTransit** (in-memory) for event bus
- **Swagger/OpenAPI** with JWT bearer auth support
