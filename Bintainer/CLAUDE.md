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

# Apply migrations
dotnet ef database update --project src/Modules/Users/Bintainer.Modules.Users.Infrastructure --startup-project src/API/Bintainer.Api
dotnet ef database update --project src/Modules/Inventory/Bintainer.Modules.Inventory.Infrastructure --startup-project src/API/Bintainer.Api
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
│   ├── Bintainer.Modules.Inventory.Domain          — Inventory, StorageUnit, Bin, Compartment
│   ├── Bintainer.Modules.Inventory.Application     — CRUD for inventories and storage units
│   ├── Bintainer.Modules.Inventory.Presentation    — Inventory endpoints (api/inventories/*, api/storage-units/*)
│   └── Bintainer.Modules.Inventory.Infrastructure  — InventoryDbContext, EF configs
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
- **Schema-per-module**: `users`, `inventory` PostgreSQL schemas

## Database

- **PostgreSQL** — database name **`BintainerDb`**, using `Npgsql.EntityFrameworkCore.PostgreSQL`
- Connection string in `src/API/Bintainer.Api/appsettings.json` under `ConnectionStrings:DefaultConnection`
- **Snake_case naming** via `EFCore.NamingConventions`
- **Two DbContexts**:
  - `UsersDbContext` (extends `IdentityDbContext<IdentityUser>`) — schema `users`
  - `InventoryDbContext` (extends `DbContext`) — schema `inventory`
- JWT config in `modules.users.json`, inventory config in `modules.inventory.json`

## DI Registration

All services registered as **scoped**. Module registration via extension methods:
- `AddApplication(assemblies)` — MediatR + behaviors + validators
- `AddInfrastructure(connectionString)` — Npgsql, caching, MassTransit, clock, auth
- `AddUsersModule(config)` — Identity, JWT, user repos
- `AddInventoryModule(config)` — Inventory DbContext, repos

## External Services

- **Serilog** for structured logging
- **MassTransit** (in-memory) for event bus
- **Swagger/OpenAPI** with JWT bearer auth support
