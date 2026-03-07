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
  compartments: Compartment[];
}

export interface Compartment {
  id: string;
  index: number;
  label: string;
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
