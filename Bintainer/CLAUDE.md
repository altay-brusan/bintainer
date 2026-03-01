# Bintainer - Claude Code Guide

## Build / Run / Test

```bash
# Restore and build
dotnet build Bintainer.sln

# Run the web app
dotnet run --project Bintainer.WebApp

# Run tests
dotnet test Bintainer.Test

# Add EF Core migration (from repo root)
dotnet ef migrations add <MigrationName> --project Bintainer.Repository --startup-project Bintainer.WebApp

# Apply migrations
dotnet ef database update --project Bintainer.Repository --startup-project Bintainer.WebApp
```

## Solution Structure

```
Bintainer.sln
├── Bintainer.Model          (.NET 6.0) - Entities, DTOs, view models, Response<T>
├── Bintainer.SharedResources (.NET 6.0) - Localization resources
├── Bintainer.Repository     (.NET 7.0) - DbContexts, repositories, EF migrations
├── Bintainer.Service        (.NET 7.0) - Business logic, AutoMapper profile
├── Bintainer.WebApp         (.NET 7.0) - Razor Pages UI, Program.cs (DI root)
├── Bintainer.Test           (.NET 7.0) - NUnit + Moq tests
└── Bintainer.Db             (SSDT)     - SQL Server database project
```

**Dependency flow:** WebApp → Service → Repository → Model. SharedResources is referenced by WebApp and Service.

## Architecture & Patterns

- **Razor Pages** (not MVC controllers or REST API). Pages live in `Bintainer.WebApp/Pages/`.
- **Repository pattern**: interfaces in Repository project (`IBinRepository`, `IPartRepository`, etc.), all registered as **scoped**.
- **Service layer**: interfaces in Service project (`IBinService`, `IPartService`, etc.), all registered as **scoped**.
- **`Response<T>`** (defined in `Bintainer.Model/Template/Response.cs`): every service method returns `Response<T>` with `IsSuccess`, `Result`, and `Message` properties.
- **AutoMapper**: `MappingProfile` in `Bintainer.Service/Helper/MappingProfile.cs` maps entities to view models.

## Database

- **SQL Server LocalDB** — instance `(localdb)\ProjectModels`, database name **`EtrekDb`**.
- Connection string in `appsettings.json` under `ConnectionStrings:DefaultConnection`.
- **Two DbContexts** in `Bintainer.Repository/`:
  - `BintainerDbContext` (extends `DbContext`) — domain entities (Bins, Parts, Orders, Inventories, etc.)
  - `ApplicationDbContext` (extends `IdentityDbContext`) — ASP.NET Identity tables
- EF Core migrations live in the Repository project.

## DI Registration (Program.cs)

All repositories and services are **scoped** except `IEmailSender` → `SESEmailSender` which is **transient**. `DigikeyService` is also scoped (Digikey API integration).

## Testing

- **NUnit 3** + **Moq** + **MockQueryable.Moq** for mocking EF Core queryables.
- Tests in `Bintainer.Test/`. The test project references all other projects.

## External Services

- **AWS SES** for email sending (`SESEmailSender`).
- **Digikey API** integration (`DigikeyService`) for part data lookup.
- **Serilog** for logging (`IAppLogger` → `AppLogger`).
