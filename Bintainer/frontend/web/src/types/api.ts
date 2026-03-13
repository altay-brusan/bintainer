export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
}

export interface AuthTokens {
  accessToken: string;
  refreshToken: string;
}

export interface Inventory {
  id: string;
  name: string;
  userId: string;
}

export interface StorageUnitSummary {
  id: string;
  name: string;
  columns: number;
  rows: number;
  compartmentCount: number;
  inventoryId: string;
}

export interface StorageUnitDetail extends StorageUnitSummary {
  bins: Bin[];
}

export interface Bin {
  id: string;
  column: number;
  row: number;
  isActive: boolean;
  compartments: Compartment[];
}

export interface Compartment {
  id: string;
  index: number;
  label: string;
  componentId?: string;
  componentPartNumber?: string;
  quantity: number;
  isActive: boolean;
}

export interface PartAttribute {
  title: string;
  value: string;
}

export interface Part {
  id: string;
  partNumber: string;
  manufacturerPartNumber: string;
  description: string;
  imageUrl: string;
  manufacturer?: string;
  detailedDescription?: string;
  footprint?: string;
  category?: string;
  url?: string;
  binLabel?: string;
  quantity: number;
  lowStockThreshold: number;
  attributes: PartAttribute[];
}

// Component types (from Catalog)

export interface ComponentLocationResponse {
  compartmentId: string;
  label: string;
  quantity: number;
  binId: string;
  storageUnitId: string;
  storageUnitName: string;
}

export interface ComponentResponse {
  id: string;
  partNumber: string;
  manufacturerPartNumber: string;
  description: string;
  detailedDescription?: string;
  imageUrl?: string;
  url?: string;
  provider?: string;
  providerPartNumber?: string;
  categoryId?: string;
  categoryName?: string;
  footprintId?: string;
  footprintName?: string;
  attributes: Record<string, string>;
  tags?: string;
  unitPrice?: number;
  manufacturer?: string;
  lowStockThreshold: number;
  locations: ComponentLocationResponse[];
}

export interface ComponentSummaryResponse {
  id: string;
  partNumber: string;
  manufacturerPartNumber: string;
  description: string;
  imageUrl?: string;
  categoryId?: string;
  categoryName?: string;
  footprintId?: string;
  footprintName?: string;
  tags?: string;
  unitPrice?: number;
  manufacturer?: string;
}

export interface SearchComponentItemResponse extends ComponentSummaryResponse {
  totalQuantity: number;
}

export interface SearchComponentsPagedResponse {
  totalCount: number;
  page: number;
  pageSize: number;
  items: SearchComponentItemResponse[];
}

// Category

export interface CategoryResponse {
  id: string;
  name: string;
  parentId?: string;
  children: CategoryResponse[];
}

// Footprint

export interface FootprintResponse {
  id: string;
  name: string;
}

// Movement

export interface MovementItemResponse {
  id: string;
  date: string;
  componentId: string;
  componentPartNumber?: string;
  action: string;
  quantity: number;
  compartmentId?: string;
  compartmentLabel?: string;
  sourceCompartmentId?: string;
  sourceCompartmentLabel?: string;
  storageUnitName?: string;
  userId: string;
  userName?: string;
  notes?: string;
}

export interface MovementsPagedResponse {
  totalCount: number;
  page: number;
  pageSize: number;
  items: MovementItemResponse[];
}

// Reports

export interface SummaryResponse {
  totalComponents: number;
  totalCategories: number;
  totalStorageUnits: number;
  occupiedCompartments: number;
  totalQuantity: number;
  totalValue: number;
  recentMovements: number;
}

export interface TopComponentResponse {
  id: string;
  partNumber: string;
  description: string;
  totalQuantity: number;
  totalValue: number;
}

export interface LowStockResponse {
  id: string;
  partNumber: string;
  description: string;
  totalQuantity: number;
  lowStockThreshold: number;
}

export interface StorageUtilizationResponse {
  storageUnitId: string;
  storageUnitName: string;
  totalCompartments: number;
  occupiedCompartments: number;
}

export interface MovementTimelineResponse {
  date: string;
  count: number;
}

export interface SupplierDistributionResponse {
  supplierName: string;
  componentCount: number;
}

export interface CategoryDistributionResponse {
  categoryName: string;
  componentCount: number;
  totalValue: number;
}

// Activity Log

export interface ActivityLogItemResponse {
  id: string;
  userId: string;
  userName?: string;
  action: string;
  entityType: string;
  entityId: string;
  entityName?: string;
  message?: string;
  details?: string;
  timestamp: string;
}

export interface ActivityLogPagedResponse {
  totalCount: number;
  page: number;
  pageSize: number;
  items: ActivityLogItemResponse[];
}

// BOM

export interface ImportBomResponse {
  importId: string;
  totalLines: number;
  matchedCount: number;
  newCount: number;
  totalValue: number;
}

export interface BomHistoryItemResponse {
  id: string;
  fileName: string;
  totalLines: number;
  matchedCount: number;
  newCount: number;
  totalValue: number;
  userId: string;
  date: string;
}

export interface BomHistoryPagedResponse {
  totalCount: number;
  page: number;
  pageSize: number;
  items: BomHistoryItemResponse[];
}
