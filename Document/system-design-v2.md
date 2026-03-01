# Bintainer v2 — System Design Document

## 1. Overview

Bintainer is an inventory management system for electronic and mechanical components in labs/workshops. This document describes the full redesign using **Clean Architecture (Modular Monolith)** with a **React SPA** frontend and **.NET 9** backend API.

### Technology Stack

| Layer | Technology |
|-------|-----------|
| Backend Runtime | .NET 9 |
| API Style | Minimal API endpoints (CQRS) |
| Database | PostgreSQL (schema-per-module) |
| ORM (writes) | EF Core 9 + Npgsql |
| ORM (reads) | Dapper (raw SQL for queries) |
| Messaging | MediatR (in-process), MassTransit (cross-module) |
| Validation | FluentValidation |
| Auth | JWT + Refresh tokens (ASP.NET Identity for user store) |
| Frontend (Web) | React + TypeScript + Vite |
| Frontend (Mobile) | React Native (future) |
| Logging | Serilog |
| Caching | Redis (optional, in-memory fallback) |

---

## 2. Naming / Terminology Decision

### Research Summary

Professional warehouse management systems (WMS) use the hierarchy: **Zone → Aisle → Rack → Shelf → Bin → Slot/Position**. Electronic component software (PartsBox, PartKeepr) uses flexible terms like "storage location" with grid-based compartments (a1, b2, etc.).

### Recommendation

| Current Name | Proposed Name | Rationale |
|-------------|--------------|-----------|
| `InventorySection` | **`StorageUnit`** | Generic enough for cabinets, drawer units, racks, shelves. "Section" is ambiguous (means "zone" in WMS). "Rack" implies a specific physical form. **StorageUnit** is neutral and professional. |
| `BinSubspace` | **`Compartment`** | Industry-standard term used in PartsBox ("compartment within a box"), WMS systems ("cell/compartment"), and general warehouse terminology. Clear and universally understood. |
| `Inventory` | **`Inventory`** | Keep as-is. Standard term. |
| `Bin` | **`Bin`** | Keep as-is. Universal WMS term. |

### Updated Physical Storage Hierarchy
```
User
 └── Inventory (1 per user, flexible for multiple)
      └── StorageUnit (e.g. "Resistor Cabinet", "IC Drawer Unit")
           ├── Columns (max bins horizontally)
           ├── Rows (max bins vertically)
           ├── CompartmentCount (partitions per bin)
           └── Bin (at Column, Row within the storage unit)
                └── Compartment (indexed partition inside a bin)
```

> **Note:** `Width` → `Columns` and `Height` → `Rows` for clarity. A storage unit grid has columns (horizontal) and rows (vertical).

---

## 3. Module Architecture

Four modules following the Evently clean architecture pattern:

```
Bintainer/
├── src/
│   ├── API/
│   │   └── Bintainer.Api/                          # Composition root, Program.cs
│   │
│   ├── Common/
│   │   ├── Bintainer.Common.Domain/                # Entity, Result<T>, Error, IDomainEvent
│   │   ├── Bintainer.Common.Application/           # ICommand/IQuery, Behaviors, IUnitOfWork
│   │   ├── Bintainer.Common.Infrastructure/        # DbConnectionFactory, Interceptors, Caching
│   │   └── Bintainer.Common.Presentation/          # IEndpoint, ApiResults, ResultExtensions
│   │
│   ├── Modules/
│   │   ├── Inventory/                              # Physical storage management
│   │   │   ├── Bintainer.Modules.Inventory.Domain/
│   │   │   ├── Bintainer.Modules.Inventory.Application/
│   │   │   ├── Bintainer.Modules.Inventory.Infrastructure/
│   │   │   └── Bintainer.Modules.Inventory.Presentation/
│   │   │
│   │   ├── Catalog/                                # Part definitions, templates, categories
│   │   │   ├── Bintainer.Modules.Catalog.Domain/
│   │   │   ├── Bintainer.Modules.Catalog.Application/
│   │   │   ├── Bintainer.Modules.Catalog.Infrastructure/
│   │   │   └── Bintainer.Modules.Catalog.Presentation/
│   │   │
│   │   ├── Stock/                                  # Part-to-bin placement, quantities
│   │   │   ├── Bintainer.Modules.Stock.Domain/
│   │   │   ├── Bintainer.Modules.Stock.Application/
│   │   │   ├── Bintainer.Modules.Stock.Infrastructure/
│   │   │   └── Bintainer.Modules.Stock.Presentation/
│   │   │
│   │   └── Procurement/                            # Orders, suppliers
│   │       ├── Bintainer.Modules.Procurement.Domain/
│   │       ├── Bintainer.Modules.Procurement.Application/
│   │       ├── Bintainer.Modules.Procurement.Infrastructure/
│   │       └── Bintainer.Modules.Procurement.Presentation/
│   │
│   └── Web/                                        # React SPA
│       └── bintainer-web/
│           ├── src/
│           │   ├── api/                            # API client (axios/fetch)
│           │   ├── components/                     # Shared UI components
│           │   ├── features/                       # Feature-based pages
│           │   │   ├── inventory/
│           │   │   ├── catalog/
│           │   │   ├── stock/
│           │   │   ├── procurement/
│           │   │   └── auth/
│           │   ├── hooks/
│           │   ├── store/                          # State management
│           │   └── types/
│           ├── package.json
│           └── vite.config.ts
│
├── tests/
│   ├── Bintainer.Modules.Inventory.Tests/
│   ├── Bintainer.Modules.Catalog.Tests/
│   ├── Bintainer.Modules.Stock.Tests/
│   ├── Bintainer.Modules.Procurement.Tests/
│   └── Bintainer.Api.IntegrationTests/
│
├── Directory.Build.props                           # net9.0, nullable, warnings
└── Bintainer.sln
```

---

## 4. Module Details

### 4.1 Inventory Module — Physical Storage

**Responsibility:** Manage the physical storage structure (inventories, storage units, bins, compartments).

**PostgreSQL Schema:** `inventory`

#### Domain Entities

```
Inventory
├── Id (Guid)
├── Name (string)
├── UserId (string)  ← owner
└── StorageUnits (collection)

StorageUnit
├── Id (Guid)
├── Name (string, e.g. "Resistor Cabinet")
├── Columns (int, max bins horizontally)
├── Rows (int, max bins vertically)
├── CompartmentCount (int, partitions per bin)
├── InventoryId (Guid)
└── Bins (collection)

Bin
├── Id (Guid)
├── Column (int, X coordinate)
├── Row (int, Y coordinate)
├── StorageUnitId (Guid)
└── Compartments (collection)

Compartment
├── Id (Guid)
├── Index (int, 0-based partition index)
├── Label (string?, physical label text)
└── BinId (Guid)
```

#### Key Commands & Queries

| Operation | Type | Description |
|-----------|------|-------------|
| `CreateInventory` | Command | Create new inventory for user |
| `CreateStorageUnit` | Command | Add storage unit with grid dimensions |
| `UpdateStorageUnit` | Command | Rename, resize grid |
| `DeleteStorageUnit` | Command | Remove storage unit (validates no stock placed) |
| `GetInventory` | Query | Get user's inventory with all storage units |
| `GetStorageUnit` | Query | Get storage unit with bins and compartments |

#### Validation Rules (replaces old SQL trigger)
- `Bin.Column >= 0 && Bin.Column < StorageUnit.Columns`
- `Bin.Row >= 0 && Bin.Row < StorageUnit.Rows`
- `Compartment.Index >= 0 && Compartment.Index < StorageUnit.CompartmentCount`

#### Domain Events
- `StorageUnitCreated` → Stock module listens (to initialize empty bins)
- `StorageUnitDeleted` → Stock module listens (to clean up placements)

---

### 4.2 Catalog Module — Part Definitions

**Responsibility:** Define parts, templates, categories, packages, groups, labels. Optionally fetch from supplier APIs.

**PostgreSQL Schema:** `catalog`

#### Domain Entities

```
PartAttributeTemplate
├── Id (Guid)
├── Name (string, e.g. "Resistor")
├── UserId (string)
└── Definitions (collection)

PartAttributeDefinition
├── Id (Guid)
├── Name (string, e.g. "Resistance")
├── DefaultValue (string?, e.g. "10k")
├── DisplayOrder (int)
└── TemplateId (Guid)

Part
├── Id (Guid)
├── Number (string, part number)
├── Description (string?)
├── CategoryId (Guid?)
├── PackageId (Guid?)
├── TemplateId (Guid?)
├── Supplier (string?)
├── ImageUri (string?)
├── DatasheetUri (string?)
├── SupplierUri (string?)
├── UserId (string)
└── Attributes (collection)
└── Labels (collection)

PartAttribute
├── Id (Guid)
├── Name (string)
├── Value (string)
└── PartId (Guid)

PartCategory
├── Id (Guid)
├── Name (string)
├── ParentCategoryId (Guid?)  ← self-referencing tree
├── UserId (string)

PartPackage
├── Id (Guid)
├── Name (string, e.g. "0805 SMD", "DIP-8")
├── UserId (string)

PartGroup
├── Id (Guid)
├── Name (string, e.g. "USB Circuit")
├── UserId (string)

PartLabel
├── Id (Guid)
├── Value (string, e.g. "10k Pot")
├── PartId (Guid)
```

#### Key Commands & Queries

| Operation | Type | Description |
|-----------|------|-------------|
| `CreateTemplate` | Command | Create attribute template with definitions |
| `UpdateTemplate` | Command | Add/remove/reorder definitions |
| `CreatePart` | Command | Create part (manually or from template) |
| `UpdatePart` | Command | Update part details and attributes |
| `DeletePart` | Command | Remove part (validates no stock) |
| `CreateCategory` | Command | Add category node to tree |
| `UpdateCategoryTree` | Command | Bulk update category hierarchy |
| `FetchSupplierData` | Command | Fetch part data from Digikey/Mouser API |
| `GetParts` | Query | List/search parts with filters |
| `GetPart` | Query | Get part with attributes, labels, category |
| `GetTemplates` | Query | List templates |
| `GetTemplateAttributes` | Query | Get template's default attributes |
| `GetCategories` | Query | Get category tree |
| `GetPackages` | Query | List packages |
| `GetGroups` | Query | List groups |
| `SearchParts` | Query | Fuzzy/similarity search across parts, labels, attributes (pg_trgm) |
| `GetPartsByLabel` | Query | Find parts by exact label match |

#### Template System Workflow (simplified)
1. User creates a `PartAttributeTemplate` (e.g. "Resistor") — **manual, always available**
2. User adds `PartAttributeDefinition`s (fields: "Resistance", "Tolerance", "Power Rating")
3. Optionally: user can auto-populate definitions from a supplier API (Digikey, Mouser)
4. When creating a `Part`, user selects a template → form auto-generates fields
5. User fills in values → stored as `PartAttribute` instances
6. **No template required** — user can create parts without templates (freeform attributes)

#### Supplier Integration (optional, per-supplier)
- `ISupplierApiClient` interface in Application layer
- `DigikeyApiClient`, `MouserApiClient` implementations in Infrastructure
- `OzdisanApiClient` — limited API, may only support basic search
- Each returns `SupplierPartData` DTO with: parameters, datasheet URL, image URL, pricing
- User explicitly triggers fetch → data populates template fields → user reviews and saves

---

### 4.3 Stock Module — Placement & Quantities

**Responsibility:** Track where parts are physically stored and in what quantities. Handle placement, removal, transfer, and quantity adjustments.

**PostgreSQL Schema:** `stock`

#### Domain Entities

```
StockPlacement (replaces PartBinAssociation)
├── Id (Guid)
├── PartId (Guid)          ← references Catalog.Part
├── BinId (Guid)           ← references Inventory.Bin
├── CompartmentId (Guid?)  ← references Inventory.Compartment (nullable = whole bin)
├── Quantity (int)
├── UserId (string)
├── PlacedAt (DateTime)
├── LastUpdatedAt (DateTime)
```

> **Note:** Stock module owns its own table but references Catalog and Inventory entities by ID (not foreign keys across schemas — cross-module references are by convention, not DB constraints).

#### Key Commands & Queries

| Operation | Type | Description |
|-----------|------|-------------|
| `PlacePart` | Command | Place quantity of a part in a bin/compartment |
| `RemovePart` | Command | Remove quantity from a placement |
| `TransferPart` | Command | Move quantity from one location to another |
| `AdjustQuantity` | Command | Set absolute quantity for a placement |
| `UsePart` | Command | Decrement quantity by 1 (quick use from workbench) |
| `BatchArrangeParts` | Command | Multiple place/transfer/remove in one transaction (drag-and-drop UI) |
| `GetStockByPart` | Query | Where is this part stored? (all locations + quantities) |
| `GetStockByBin` | Query | What's in this bin? (all parts + quantities) |
| `GetStockByStorageUnit` | Query | Grid view of a storage unit with contents |
| `GetStockSummary` | Query | Total inventory value/count dashboard |
| `FindPart` | Query | Search: "where is part X?" → returns bin locations |

#### Domain Events
- `PartPlaced` → could trigger low-stock alerts in future
- `PartRemoved` → could trigger reorder notifications
- `QuantityAdjusted` → audit trail

---

### 4.4 Procurement Module — Orders & Suppliers

**Responsibility:** Track purchase orders from suppliers, link ordered items to catalog parts, track delivery status.

**PostgreSQL Schema:** `procurement`

#### Domain Entities

```
Supplier
├── Id (Guid)
├── Name (string, e.g. "Digikey", "Mouser", "Ozdisan")
├── WebsiteUrl (string?)
├── ApiEnabled (bool)
├── UserId (string)

Order
├── Id (Guid)
├── OrderNumber (string)
├── SupplierId (Guid)
├── Status (enum: Draft, Placed, Shipped, Delivered, Cancelled)
├── OrderDate (DateTime?)
├── DeliveryDate (DateTime?)
├── Notes (string?)
├── UserId (string)
└── Items (collection)

OrderItem
├── Id (Guid)
├── PartId (Guid)       ← references Catalog.Part
├── Quantity (int)
├── UnitPrice (decimal?)
├── Currency (string?)
├── OrderId (Guid)
```

#### Key Commands & Queries

| Operation | Type | Description |
|-----------|------|-------------|
| `CreateOrder` | Command | Create new purchase order |
| `AddOrderItem` | Command | Add part to order |
| `UpdateOrderStatus` | Command | Change order status |
| `ReceiveOrder` | Command | Mark as delivered → optionally auto-place in stock |
| `GetOrders` | Query | List orders with filters |
| `GetOrder` | Query | Get order with items |
| `GetSuppliers` | Query | List suppliers |
| `GetOrderHistory` | Query | Order history for a specific part |

#### Integration Events
- `OrderReceived` → Stock module listens (to auto-suggest placement)
- `OrderItemAdded` → Could validate part exists in Catalog

---

## 5. Common Layer

Following the Evently pattern exactly:

### Bintainer.Common.Domain
- `Entity` base class (Id, DomainEvents collection, Raise() method)
- `Result` / `Result<T>` (railway-oriented, Success/Failure)
- `Error` record (Code, Description, ErrorType)
- `ErrorType` enum (Failure, Validation, Problem, NotFound, Conflict)
- `ValidationError` (wraps multiple Error instances)
- `IDomainEvent` / `DomainEvent` base class

### Bintainer.Common.Application
- `ICommand` / `ICommand<T>` (wraps MediatR IRequest)
- `ICommandHandler<TCommand>` / `ICommandHandler<TCommand, TResponse>`
- `IQuery<T>` / `IQueryHandler<TQuery, TResponse>`
- `IDomainEventHandler<T>`
- Pipeline behaviors: `ValidationPipelineBehavior`, `ExceptionHandlingPipelineBehavior`, `RequestLoggingPipelineBehavior`
- `IUnitOfWork` (SaveChangesAsync)
- `IDbConnectionFactory` (for Dapper queries)
- `IDateTimeProvider`
- `IEventBus` + `IIntegrationEvent`

### Bintainer.Common.Infrastructure
- `DbConnectionFactory` (Npgsql)
- `PublishDomainEventsInterceptor` (EF Core SaveChangesInterceptor)
- `DateTimeProvider`
- `EventBus` (MassTransit wrapper)
- Caching services (Redis/in-memory)

### Bintainer.Common.Presentation
- `IEndpoint` interface + `EndpointExtensions` (auto-discovery)
- `ApiResults` (Error → RFC 7807 ProblemDetails)
- `ResultExtensions` (Match pattern)

---

## 6. Authentication & Authorization

### Backend (JWT)
- ASP.NET Identity for user store (in a dedicated `users` schema)
- JWT access tokens (short-lived, ~15 min)
- Refresh tokens (long-lived, stored in DB)
- Endpoints: `POST /api/auth/register`, `POST /api/auth/login`, `POST /api/auth/refresh`, `POST /api/auth/logout`
- All module endpoints require `[Authorize]` (via middleware)
- `UserId` extracted from JWT claims in endpoints, passed to commands/queries

### Frontend (React)
- Tokens stored in memory (access) + httpOnly cookie (refresh)
- Axios interceptor auto-refreshes expired tokens
- Protected routes with auth guard component

---

## 7. React SPA Architecture

```
bintainer-web/
├── public/
├── src/
│   ├── api/
│   │   ├── client.ts              # Axios instance with interceptors
│   │   ├── inventoryApi.ts
│   │   ├── catalogApi.ts
│   │   ├── stockApi.ts
│   │   └── procurementApi.ts
│   │
│   ├── components/
│   │   ├── Layout/                # Shell, Sidebar, Header
│   │   ├── Common/                # Button, Modal, Table, Form, TagInput, SearchBar, etc.
│   │   ├── StorageGrid/           # Reusable bin-grid visualizer (table + highlight support)
│   │   └── DragDrop/              # Drag-and-drop wrappers (@dnd-kit/core)
│   │
│   ├── features/
│   │   ├── auth/
│   │   │   ├── LoginPage.tsx
│   │   │   ├── RegisterPage.tsx
│   │   │   └── useAuth.ts
│   │   │
│   │   ├── inventory/
│   │   │   ├── InventoryPage.tsx          # Main inventory view
│   │   │   ├── StorageUnitEditor.tsx      # Create/edit storage unit
│   │   │   └── StorageUnitGrid.tsx        # Visual grid of bins
│   │   │
│   │   ├── catalog/
│   │   │   ├── PartsPage.tsx              # Part list/search
│   │   │   ├── PartEditor.tsx             # Create/edit part
│   │   │   ├── TemplatesPage.tsx          # Template management
│   │   │   ├── TemplateEditor.tsx         # Create/edit template
│   │   │   ├── CategoriesPage.tsx         # Category tree editor
│   │   │   └── SupplierLookup.tsx         # Fetch from Digikey/Mouser
│   │   │
│   │   ├── stock/
│   │   │   ├── StockDashboard.tsx         # Overview, totals
│   │   │   ├── PlacePartModal.tsx         # Place part in bin
│   │   │   ├── FindPartPage.tsx           # "Where is part X?" (card + grid highlight)
│   │   │   ├── FindResultCard.tsx         # Card showing location (unit, col, row, qty)
│   │   │   ├── BinContentsView.tsx        # "What's in this bin?"
│   │   │   ├── ArrangeMode.tsx            # Drag-and-drop grid wrapper
│   │   │   ├── DraggablePartChip.tsx      # Part item that can be dragged
│   │   │   └── DroppableBinCell.tsx        # Bin cell that accepts drops
│   │   │
│   │   ├── procurement/
│   │   │   ├── OrdersPage.tsx             # Order list
│   │   │   ├── OrderEditor.tsx            # Create/edit order
│   │   │   └── SuppliersPage.tsx          # Manage suppliers
│   │   │
│   │   └── dashboard/
│   │       └── DashboardPage.tsx          # Home page with summary
│   │
│   ├── hooks/
│   │   ├── useApi.ts                      # Generic fetch hook
│   │   └── useDebounce.ts
│   │
│   ├── store/                             # Zustand or React Query
│   │   └── ...
│   │
│   ├── types/                             # TypeScript interfaces (mirror API DTOs)
│   │   ├── inventory.ts
│   │   ├── catalog.ts
│   │   ├── stock.ts
│   │   └── procurement.ts
│   │
│   ├── App.tsx
│   ├── Router.tsx
│   └── main.tsx
│
├── index.html
├── package.json
├── tsconfig.json
└── vite.config.ts
```

### Key UI Pages (mapping from existing Razor pages)

| Existing Razor Page | New React Page | Module |
|---------------------|---------------|--------|
| `Dashboard/Inventory.cshtml` | `InventoryPage.tsx` + `StorageUnitGrid.tsx` | Inventory |
| `Dashboard/Template.cshtml` | `TemplatesPage.tsx` + `CategoriesPage.tsx` | Catalog |
| `Dashboard/Part.cshtml` | `PartsPage.tsx` + `PartEditor.tsx` | Catalog |
| `Dashboard/Bin.cshtml` | `BinContentsView.tsx` + `PlacePartModal.tsx` | Stock |
| `Dashboard/Order.cshtml` | `OrdersPage.tsx` + `OrderEditor.tsx` | Procurement |
| `About.cshtml` | `DashboardPage.tsx` (or separate About) | — |
| Identity pages | `LoginPage.tsx` + `RegisterPage.tsx` | Auth |

---

## 8. API Endpoints Summary

### Inventory Module (`/api/inventory`)
```
GET    /api/inventory                    → GetInventory
POST   /api/inventory/storage-units      → CreateStorageUnit
PUT    /api/inventory/storage-units/{id} → UpdateStorageUnit
DELETE /api/inventory/storage-units/{id} → DeleteStorageUnit
GET    /api/inventory/storage-units/{id} → GetStorageUnit
```

### Catalog Module (`/api/catalog`)
```
GET    /api/catalog/parts                → GetParts (with filter: ?label=X, ?category=X, ?group=X)
GET    /api/catalog/parts/{id}           → GetPart
POST   /api/catalog/parts                → CreatePart
PUT    /api/catalog/parts/{id}           → UpdatePart
DELETE /api/catalog/parts/{id}           → DeletePart
GET    /api/catalog/parts/search?q=...&threshold=0.3 → SearchParts (fuzzy/similarity via pg_trgm)

GET    /api/catalog/templates            → GetTemplates
GET    /api/catalog/templates/{id}       → GetTemplateAttributes
POST   /api/catalog/templates            → CreateTemplate
PUT    /api/catalog/templates/{id}       → UpdateTemplate

GET    /api/catalog/categories           → GetCategories (tree)
POST   /api/catalog/categories           → CreateCategory
PUT    /api/catalog/categories           → UpdateCategoryTree

GET    /api/catalog/packages             → GetPackages
GET    /api/catalog/groups               → GetGroups

POST   /api/catalog/supplier-lookup      → FetchSupplierData
```

### Stock Module (`/api/stock`)
```
POST   /api/stock/place                  → PlacePart
POST   /api/stock/remove                 → RemovePart
POST   /api/stock/transfer               → TransferPart
POST   /api/stock/adjust                 → AdjustQuantity
POST   /api/stock/use                    → UsePart (quick -1)
POST   /api/stock/batch-arrange          → BatchArrangeParts (multiple moves in one tx, for drag-and-drop UI)

GET    /api/stock/by-part/{partId}       → GetStockByPart
GET    /api/stock/by-bin/{binId}         → GetStockByBin
GET    /api/stock/by-unit/{unitId}       → GetStockByStorageUnit (grid view with contents)
GET    /api/stock/summary                → GetStockSummary
GET    /api/stock/find?q=...             → FindPart (returns card data + grid highlight coordinates)
```

### Procurement Module (`/api/procurement`)
```
GET    /api/procurement/orders           → GetOrders
GET    /api/procurement/orders/{id}      → GetOrder
POST   /api/procurement/orders           → CreateOrder
PUT    /api/procurement/orders/{id}      → UpdateOrder
POST   /api/procurement/orders/{id}/items → AddOrderItem
PUT    /api/procurement/orders/{id}/status → UpdateOrderStatus
POST   /api/procurement/orders/{id}/receive → ReceiveOrder

GET    /api/procurement/suppliers        → GetSuppliers
POST   /api/procurement/suppliers        → CreateSupplier
```

### Auth (`/api/auth`)
```
POST   /api/auth/register               → Register
POST   /api/auth/login                   → Login (returns JWT + refresh)
POST   /api/auth/refresh                 → RefreshToken
POST   /api/auth/logout                  → Logout (revoke refresh)
GET    /api/auth/me                      → GetCurrentUser
```

---

## 9. Database Schema Layout

```
PostgreSQL Database: BintainerDb
├── inventory schema
│   ├── inventories
│   ├── storage_units
│   ├── bins
│   └── compartments
│
├── catalog schema
│   ├── parts
│   ├── part_attributes
│   ├── part_attribute_templates
│   ├── part_attribute_definitions
│   ├── part_categories
│   ├── part_packages
│   ├── part_groups
│   ├── part_group_memberships (part_id, group_id)
│   └── part_labels
│
├── stock schema
│   └── stock_placements
│
├── procurement schema
│   ├── suppliers
│   ├── orders
│   └── order_items
│
└── users schema
    ├── asp_net_users (Identity)
    ├── asp_net_roles
    ├── asp_net_user_roles
    └── refresh_tokens
```

> Snake_case naming convention via `UseSnakeCaseNamingConvention()` (EFCore.NamingConventions package).

### PostgreSQL Extensions
```sql
CREATE EXTENSION IF NOT EXISTS pg_trgm;  -- Trigram similarity for fuzzy search
```

### Indexes for Fuzzy Search
```sql
CREATE INDEX idx_parts_number_trgm ON catalog.parts USING gin (number gin_trgm_ops);
CREATE INDEX idx_parts_description_trgm ON catalog.parts USING gin (description gin_trgm_ops);
CREATE INDEX idx_part_labels_value_trgm ON catalog.part_labels USING gin (value gin_trgm_ops);
CREATE INDEX idx_part_attributes_value_trgm ON catalog.part_attributes USING gin (value gin_trgm_ops);
```

---

## 10. Cross-Module Communication

| From | To | Mechanism | Event |
|------|-----|-----------|-------|
| Inventory | Stock | Integration Event | `StorageUnitDeleted` → clean up placements |
| Procurement | Stock | Integration Event | `OrderReceived` → suggest auto-placement |
| Catalog | Stock | Sync (PublicApi) | Stock queries need part names/details |
| Catalog | Procurement | Sync (PublicApi) | Orders reference parts by ID |
| Inventory | Stock | Sync (PublicApi) | Stock queries need bin/unit details |

**Cross-module references are by ID only** — no shared foreign keys across schemas. Each module owns its data.

---

## 11. Implementation Order

### Phase 1: Foundation
1. Create solution structure with `Directory.Build.props` (net9.0)
2. Implement `Common` layer (Domain, Application, Infrastructure, Presentation)
3. Set up `Bintainer.Api` project with Program.cs, global exception handler, Serilog
4. Set up JWT authentication with ASP.NET Identity
5. Set up React project with Vite, routing, auth pages

### Phase 2: Inventory Module
6. Implement Inventory module (Domain → Application → Infrastructure → Presentation)
7. Create React inventory pages (StorageUnit CRUD, grid visualizer)

### Phase 3: Catalog Module
8. Implement Catalog module (templates, parts, categories, packages, groups)
9. Create React catalog pages (parts list, editor, templates, categories)
10. Implement supplier integration (Digikey first, then others)

### Phase 4: Stock Module
11. Implement Stock module (placements, quantities, find/search)
12. Create React stock pages (dashboard, place/remove, find part)
13. Wire cross-module events (Inventory → Stock)

### Phase 5: Procurement Module
14. Implement Procurement module (orders, suppliers)
15. Create React procurement pages (orders, suppliers)
16. Wire cross-module events (Procurement → Stock)

### Phase 6: Polish
17. Dashboard page with summary stats
18. Mobile-friendly responsive design
19. Performance optimization (caching, query optimization)
20. React Native mobile app (future)

---

## 12. Use Cases & UI Behavior

### UC1: Create Inventory Structure
**Flow:** User creates Inventory → adds StorageUnits → bins/compartments auto-generated from grid dimensions.

**Backend:**
- `CreateInventory` → creates empty inventory
- `CreateStorageUnit(name, columns, rows, compartmentCount)` → auto-creates `columns × rows` Bin entities, each with `compartmentCount` Compartment entities
- `UpdateStorageUnit` → can resize grid (add/remove bins), rename
- `DeleteStorageUnit` → validates no stock placements exist first

**Frontend:**
- `InventoryPage.tsx` — shows all storage units as cards/tiles
- `StorageUnitEditor.tsx` — form to set name, columns, rows, compartmentCount
- After creating a storage unit, user sees the grid immediately and can customize bin labels

**Key:** When `CreateStorageUnit` command runs, the handler auto-generates all Bin + Compartment entities in one transaction. User does NOT manually create each bin.

---

### UC2: View StorageUnit as Table/Grid
**Flow:** User clicks on a storage unit → sees a table/grid view where rows and columns represent physical bin positions.

**Backend:**
- `GetStockByStorageUnit(unitId)` → returns full grid with bin contents:
  ```json
  {
    "storageUnit": { "name": "Resistor Cabinet", "columns": 5, "rows": 4 },
    "grid": [
      { "column": 0, "row": 0, "binId": "...", "parts": [
        { "partNumber": "RC0805", "name": "10k Resistor", "quantity": 50, "compartment": 0 }
      ]},
      ...
    ]
  }
  ```

**Frontend:**
- `StorageUnitGrid.tsx` — renders an HTML table where each cell = a bin
- Each cell shows: part count, total quantity, color coding (empty/occupied/low stock)
- Click on a cell → expands to show compartments and part details
- Responsive: scrollable on small screens

---

### UC3: Find Component — Card View + Graphical Highlight
**Flow:** User searches for a part → results show both a location card AND a visual grid highlight.

**Backend:**
- `FindPart(query)` → returns list of matches with location info:
  ```json
  {
    "results": [{
      "partId": "...",
      "partNumber": "RC0805",
      "partName": "10k Resistor",
      "locations": [{
        "storageUnitId": "...",
        "storageUnitName": "Resistor Cabinet",
        "column": 2,
        "row": 3,
        "compartmentIndex": 0,
        "compartmentLabel": "10k Pot",
        "quantity": 50
      }]
    }]
  }
  ```

**Frontend:**
- **Card view:** Shows part info + location as text: "Resistor Cabinet → Column 2, Row 3, Compartment 0 (50 pcs)"
- **Grid view:** Renders the `StorageUnitGrid` component with the target bin highlighted (pulsing/colored border)
- Toggle between card and grid views, or show both side-by-side on desktop
- Component: `FindPartPage.tsx` → `FindResultCard.tsx` + `StorageUnitGrid.tsx` (with `highlightBin` prop)

---

### UC4: Drag-and-Drop Part Arrangement
**Flow:** User drags parts between bins/compartments visually on the grid.

**Backend:**
- `TransferPart(partId, fromBinId, fromCompartmentId, toBinId, toCompartmentId, quantity)` — already planned
- `PlacePart` / `RemovePart` for placing new or removing entirely
- New endpoint: `POST /api/stock/batch-arrange` → `BatchArrangeParts` command for multiple moves in one transaction

**Frontend:**
- `StorageUnitGrid.tsx` enhanced with drag-and-drop mode (toggle via button)
- Uses a library like `@dnd-kit/core` (React) or `react-beautiful-dnd`
- Drag a part chip from one cell to another → calls `TransferPart`
- Drag from part list (sidebar) onto a cell → calls `PlacePart`
- Visual feedback: drop targets highlight, ghost preview during drag
- **Component structure:**
  ```
  stock/
  ├── ArrangeMode.tsx           # Drag-and-drop grid wrapper
  ├── DraggablePartChip.tsx     # Part item that can be dragged
  └── DroppableBinCell.tsx      # Bin cell that accepts drops
  ```
- **Note:** We will review the exact UX in detail during implementation. This is the structural foundation.

---

### UC5: Similarity Search (Fuzzy Matching)
**Flow:** User types a partial or misspelled part name/number → system returns similar matches.

**Backend:**
- PostgreSQL `pg_trgm` extension for trigram-based similarity:
  ```sql
  CREATE EXTENSION IF NOT EXISTS pg_trgm;
  CREATE INDEX idx_parts_number_trgm ON catalog.parts USING gin (number gin_trgm_ops);
  CREATE INDEX idx_parts_description_trgm ON catalog.parts USING gin (description gin_trgm_ops);
  ```
- `SearchParts(query, threshold)` query (Dapper) uses similarity scoring:
  ```sql
  SELECT p.*, similarity(p.number, @query) AS score
  FROM catalog.parts p
  WHERE p.number % @query
     OR p.description % @query
     OR EXISTS (SELECT 1 FROM catalog.part_labels pl WHERE pl.part_id = p.id AND pl.value % @query)
     OR EXISTS (SELECT 1 FROM catalog.part_attributes pa WHERE pa.part_id = p.id AND pa.value % @query)
  ORDER BY score DESC
  LIMIT 20
  ```
- The `%` operator returns true when similarity exceeds `pg_trgm.similarity_threshold` (default 0.3)
- Also search across: part number, description, labels, attribute values

**API:**
```
GET /api/catalog/parts/search?q=10k+restor&threshold=0.3
```

**Frontend:**
- Search bar with debounced input (300ms)
- Results show similarity score as a visual indicator
- "Did you mean?" suggestions when exact match not found

---

### UC6: Labels as Searchable Metadata
**Flow:** User puts labels on components (e.g., "10k Pot" on part "J50S 10K"). Labels are searchable metadata.

**Backend:**
- `PartLabel` entity already planned with `Value` field
- Labels are included in `SearchParts` fuzzy search (see UC5 SQL above)
- Labels included in `FindPart` response
- New: `GetPartsByLabel(label)` query for exact label-based lookup

**API:**
```
GET /api/catalog/parts?label=10k+Pot        → exact label filter
GET /api/catalog/parts/search?q=10k+Pot     → fuzzy search (includes labels)
```

**Frontend:**
- Part editor shows label management (add/remove labels as tags)
- Search results show labels as colored chips/badges
- Filter-by-label option in parts list

---

## 13. ID Strategy: Guid Everywhere

All entity IDs use `Guid` (no `int` primary keys). This applies to:

| Entity | Old ID Type | New ID Type |
|--------|------------|-------------|
| `Inventory` | int | **Guid** |
| `StorageUnit` | int | **Guid** |
| `Bin` | int | **Guid** |
| `Compartment` | int | **Guid** |
| `Part` | int | **Guid** |
| `PartCategory` | int | **Guid** |
| `PartPackage` | int | **Guid** |
| `PartGroup` | Guid | Guid (already) |
| `PartAttributeTemplate` | int | **Guid** |
| `PartAttributeDefinition` | int | **Guid** |
| `PartAttribute` | int | **Guid** |
| `PartLabel` | Guid | Guid (already) |
| `StockPlacement` | composite | **Guid** |
| `Supplier` | Guid | Guid (already) |
| `Order` | int | **Guid** |
| `OrderItem` | int | **Guid** |

**Benefits:**
- No sequential ID leakage (security)
- Safe for cross-module references (no ID collisions)
- Can generate IDs client-side (useful for offline/React Native)
- Consistent pattern across all entities

**Implementation:**
- Entity base class: `public abstract class Entity { public Guid Id { get; protected set; } }`
- PostgreSQL default: `HasDefaultValueSql("gen_random_uuid()")`
- EF Core config: `HasKey(e => e.Id)` on each entity

**Trade-off:** Guid PKs are slightly larger than int (16 bytes vs 4 bytes) and can cause index fragmentation. Mitigated by using PostgreSQL's `gen_random_uuid()` (v4, random) which distributes evenly, and the dataset size (component inventory) is small enough that this is negligible.

---

## 14. Key Design Principles

1. **Simplicity first** — Manual workflows must be easy. API integrations are optional enhancements.
2. **Module independence** — Each module can be developed and tested in isolation.
3. **CQRS separation** — Commands use EF Core (rich domain model), Queries use Dapper (fast reads).
4. **No exceptions for business errors** — Use `Result<T>` pattern throughout.
5. **Domain events for side effects** — Never call across modules directly for writes.
6. **Schema isolation** — Each module owns its PostgreSQL schema.
7. **Frontend mirrors backend** — React feature folders map 1:1 to backend modules.
