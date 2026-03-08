# Bintainer API Endpoints

> All endpoints require JWT Bearer auth unless noted. All responses follow the `Result<T>` pattern.
> The API is split across three modules: **Users** (auth), **Inventory** (storage structure + movements), and **Catalog** (components + categories + footprints).

---

## 1. Authentication [EXISTS — Users Module]

| Method | Path | Request Body | Description |
|--------|------|-------------|-------------|
| POST | `/api/auth/register` | `{ email, password, firstName, lastName }` | Register new user |
| POST | `/api/auth/login` | `{ email, password }` | Login, returns JWT + refresh token |
| POST | `/api/auth/refresh` | `{ refreshToken }` | Refresh access token |
| POST | `/api/auth/logout` | — | Logout (revoke refresh token) |
| GET | `/api/auth/me` | — | Get current user profile |
| PUT | `/api/auth/profile` | `{ firstName, lastName }` | Update profile |
| GET | `/api/auth/preferences` | — | Get user preferences |
| PUT | `/api/auth/preferences` | `{ currency }` | Save user preferences |

---

## 2. Inventories [EXISTS — Inventory Module]

| Method | Path | Request Body | Description |
|--------|------|-------------|-------------|
| POST | `/api/inventories` | `{ name }` | Create inventory |
| GET | `/api/inventories` | — | List user's inventories |
| GET | `/api/inventories/{inventoryId}` | — | Get inventory detail |

---

## 3. Storage Units [EXISTS — Inventory Module]

| Method | Path | Request Body | Description |
|--------|------|-------------|-------------|
| POST | `/api/storage-units` | `{ name, columns, rows, compartmentCount, inventoryId }` | Create storage unit |
| GET | `/api/inventories/{inventoryId}/storage-units` | — | List storage units in inventory |
| GET | `/api/storage-units/{storageUnitId}` | — | Get detail with bins/compartments |
| PUT | `/api/storage-units/{storageUnitId}` | `{ name }` | Update storage unit name |
| DELETE | `/api/storage-units/{storageUnitId}` | — | Delete storage unit |

---

## 4. Compartments [EXISTS — Inventory Module]

| Method | Path | Request Body | Description |
|--------|------|-------------|-------------|
| PUT | `/api/compartments/{compartmentId}/component` | `{ componentId, quantity }` | Assign component to compartment |
| DELETE | `/api/compartments/{compartmentId}/component` | — | Remove component from compartment |
| PUT | `/api/compartments/{compartmentId}/label` | `{ label }` | Update compartment label |

---

## 5. Components — Quantity & Move [EXISTS — Inventory Module]

| Method | Path | Request Body | Description |
|--------|------|-------------|-------------|
| POST | `/api/components/{componentId}/quantity` | `{ compartmentId, action, quantity, notes? }` | Adjust quantity |
| POST | `/api/components/{componentId}/move` | `{ sourceCompartmentId, destinationCompartmentId, quantity }` | Move between compartments |

`action` values: `"added"`, `"used"`, `"restocked"`

---

## 6. Movement History [EXISTS — Inventory Module]

### `GET /api/movements`

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| `action` | string? | — | Filter by action (added/used/restocked/moved) |
| `componentId` | guid? | — | Filter by specific component |
| `q` | string? | — | Search by component name |
| `page` | int | 1 | Page number |
| `pageSize` | int | 20 | Items per page |

---

## 7. Reports / Analytics [EXISTS — Inventory Module]

| Method | Path | Params | Description |
|--------|------|--------|-------------|
| GET | `/api/reports/summary` | — | Dashboard KPIs (total qty, unique parts, value, low stock count, etc.) |
| GET | `/api/reports/category-distribution` | — | Count & value by category |
| GET | `/api/reports/supplier-distribution` | — | Count by supplier/provider |
| GET | `/api/reports/movement-timeline` | `days` (default 7) | Daily movement aggregates |
| GET | `/api/reports/storage-utilization` | — | Utilization % per storage unit |
| GET | `/api/reports/top-components` | `sortBy`, `limit` (default 10) | Top components by value/qty/unitPrice |
| GET | `/api/reports/low-stock` | — | Components at/below threshold |

---

## 8. Components — CRUD [EXISTS — Catalog Module]

| Method | Path | Description |
|--------|------|-------------|
| POST | `/api/components` | Create component |
| GET | `/api/components?categoryId=` | List components (optional category filter) |
| GET | `/api/components/{componentId}` | Get component detail |
| PUT | `/api/components/{componentId}` | Update component |
| DELETE | `/api/components/{componentId}` | Delete component |

### `POST /api/components` — Request

```json
{
  "partNumber": "RC0603FR-0710KL",
  "manufacturerPartNumber": "RC0603FR-0710KL",
  "description": "10k Resistor 0603 SMD",
  "detailedDescription": "RES 10K OHM 1% 1/10W 0603",
  "manufacturer": "Yageo",
  "unitPrice": 0.01,
  "lowStockThreshold": 10,
  "tags": "smd,basic,passive",
  "categoryId": "guid-or-null",
  "footprintId": "guid-or-null",
  "provider": "Digikey",
  "providerPartNumber": "311-10.0KHRCT-ND",
  "url": "https://www.digikey.com/...",
  "imageUrl": null,
  "attributes": {
    "Resistance": "10k Ohm",
    "Tolerance": "1%",
    "Power": "1/10W"
  }
}
```

> **Note:** `tags` is a comma-separated string (max 500 chars), not an array.

---

## 9. Component Search [EXISTS — Catalog Module]

### `GET /api/components/search`

Full-text search across components with filters and pagination.

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| `q` | string? | — | Search text (matches part_number, manufacturer_part_number, description) |
| `categoryId` | guid? | — | Filter by category |
| `provider` | string? | — | Filter by supplier/provider name |
| `tag` | string? | — | Filter by tag (ILIKE match on tags column) |
| `footprintId` | guid? | — | Filter by footprint |
| `page` | int | 1 | Page number |
| `pageSize` | int | 20 | Items per page |

---

## 10. Tags Autocomplete [EXISTS — Catalog Module]

### `GET /api/tags`

Returns all unique tags used across components (for autocomplete/filter dropdowns).

**Response:**
```json
["smd", "basic", "power", "decoupling", "through-hole"]
```

---

## 11. Component Image Upload [EXISTS — Catalog Module]

### `POST /api/components/{componentId}/image`

Upload component image. Content-Type: `multipart/form-data`.

**Request:** Form field `file` (image/png, image/jpeg)

Saves to local filesystem via `LocalFileStorageService`, updates the component's `ImageUrl`.

---

## 12. Categories [EXISTS — Catalog Module]

| Method | Path | Request Body | Description |
|--------|------|-------------|-------------|
| POST | `/api/categories` | `{ name, parentId? }` | Create category (supports hierarchy) |
| GET | `/api/categories` | — | List all categories |
| PUT | `/api/categories/{categoryId}` | `{ name, parentId? }` | Update category |
| DELETE | `/api/categories/{categoryId}` | — | Delete category |

---

## 13. Footprints [EXISTS — Catalog Module]

| Method | Path | Request Body | Description |
|--------|------|-------------|-------------|
| POST | `/api/footprints` | `{ name }` | Create footprint |
| GET | `/api/footprints` | — | List footprints |
| PUT | `/api/footprints/{footprintId}` | `{ name }` | Update footprint |
| DELETE | `/api/footprints/{footprintId}` | — | Delete footprint |

---

## 14. BOM Import [EXISTS — Catalog Module]

### `POST /api/bom/import`

Import components from a parsed BOM file.

**Request:**
```json
{
  "fileName": "project-alpha-rev3.csv",
  "lines": [
    { "partNumber": "RC0603FR-0710KL", "quantity": 20, "description": "10k Resistor 0603" },
    { "partNumber": "TPS63020DSJR", "quantity": 3, "description": "Buck-Boost Converter" }
  ]
}
```

### `GET /api/bom/history`

List past BOM import records.

| Param | Type | Default | Description |
|-------|------|---------|-------------|
| `page` | int? | 1 | Page number |
| `pageSize` | int? | 20 | Items per page |

---

## 15. Not Yet Implemented

| Feature | Endpoints | Notes |
|---------|-----------|-------|
| Supplier search | `GET /api/suppliers/search` | External catalog lookup (DigiKey, Mouser, LCSC) — requires API keys |
| Supplier import | `POST /api/suppliers/import` | Import from supplier search results |
| Server-side export | `POST /api/exports/inventory` | XLSX/PDF export — currently done client-side |

---

## Module Ownership

| Module | Schema | Endpoints |
|--------|--------|-----------|
| **Users** | `users` | `/api/auth/*` |
| **Inventory** | `inventory` | `/api/inventories/*`, `/api/storage-units/*`, `/api/compartments/*`, `/api/components/{id}/quantity`, `/api/components/{id}/move`, `/api/movements`, `/api/reports/*` |
| **Catalog** | `inventory` | `/api/components` (CRUD + search + image), `/api/categories/*`, `/api/footprints/*`, `/api/tags`, `/api/bom/*` |

---

## Domain Model

```
Inventory (top-level, owned by a User)
  └── StorageUnit (physical shelf/cabinet — Name, Columns, Rows, CompartmentCount)
        └── Bin (grid cell — Column, Row)
              └── Compartment (sub-slot — Index, Label, ComponentId?, Quantity)

Component (catalog entry — PartNumber, Description, Manufacturer, UnitPrice, Tags, Attributes, LowStockThreshold, CategoryId?, FootprintId?)
Category (hierarchical grouping — Name, ParentId?)
Footprint (package type — Name)
Movement (activity log — ComponentId, Action, Quantity, Date, UserId, Notes)
```

**Relationship:** A Compartment references a Component via `ComponentId`. A Component can be in multiple Compartments (multiple storage locations).
