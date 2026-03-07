import type { StorageUnitSummary } from "@/types/api";

export interface Component {
  id: string;
  name: string;
  category: string;
  storageUnit: string;
  bin: string;
  compartment: string;
  quantity: number;
  package?: string;
  supplier?: string;
  datasheetUrl?: string;
  lowStockThreshold: number;
  tags?: string[];
}

export interface MovementRecord {
  id: string;
  date: string;
  component: string;
  action: "added" | "used" | "restocked" | "moved";
  quantity: number;
  location: string;
  user: string;
}

export const demoStorageUnits: StorageUnitSummary[] = [
  { id: "su-1", name: "Resistors", columns: 10, rows: 8, compartmentCount: 672, inventoryId: "inv-1" },
  { id: "su-2", name: "Capacitors", columns: 8, rows: 6, compartmentCount: 310, inventoryId: "inv-1" },
  { id: "su-3", name: "Inductors", columns: 6, rows: 5, compartmentCount: 150, inventoryId: "inv-1" },
  { id: "su-4", name: "Transistors", columns: 6, rows: 5, compartmentCount: 190, inventoryId: "inv-1" },
  { id: "su-5", name: "Microcontrollers", columns: 5, rows: 4, compartmentCount: 49, inventoryId: "inv-1" },
];

export const demoComponents: Component[] = [
  { id: "c-1", name: "10k Resistor", category: "Resistors", storageUnit: "Resistors", bin: "R03-C02", compartment: "P01", quantity: 120, package: "0603", supplier: "Digikey", lowStockThreshold: 20, tags: ["smd", "basic"] },
  { id: "c-2", name: "4.7k Resistor", category: "Resistors", storageUnit: "Resistors", bin: "R03-C03", compartment: "P01", quantity: 85, package: "0603", supplier: "Digikey", lowStockThreshold: 20, tags: ["smd", "basic"] },
  { id: "c-3", name: "1k Resistor", category: "Resistors", storageUnit: "Resistors", bin: "R01-C01", compartment: "P01", quantity: 200, package: "0603", supplier: "Mouser", lowStockThreshold: 30, tags: ["smd"] },
  { id: "c-4", name: "100nF Capacitor", category: "Capacitors", storageUnit: "Capacitors", bin: "R01-C01", compartment: "P01", quantity: 150, package: "0402", supplier: "Digikey", lowStockThreshold: 25, tags: ["decoupling", "smd"] },
  { id: "c-5", name: "10uF Capacitor", category: "Capacitors", storageUnit: "Capacitors", bin: "R02-C01", compartment: "P01", quantity: 60, package: "0805", supplier: "Mouser", lowStockThreshold: 15, tags: ["power"] },
  { id: "c-6", name: "22pF Capacitor", category: "Capacitors", storageUnit: "Capacitors", bin: "R01-C03", compartment: "P01", quantity: 90, package: "0402", supplier: "LCSC", lowStockThreshold: 20 },
  { id: "c-7", name: "10uH Inductor", category: "Inductors", storageUnit: "Inductors", bin: "R01-C01", compartment: "P01", quantity: 45, package: "0805", supplier: "Digikey", lowStockThreshold: 10, tags: ["power", "smd"] },
  { id: "c-8", name: "STM32F103", category: "Microcontrollers", storageUnit: "Microcontrollers", bin: "R01-C01", compartment: "P02", quantity: 14, package: "LQFP-48", supplier: "Mouser", lowStockThreshold: 5, tags: ["mcu", "arm", "project-alpha"] },
  { id: "c-9", name: "ESP32-S3", category: "Microcontrollers", storageUnit: "Microcontrollers", bin: "R01-C02", compartment: "P01", quantity: 8, package: "QFN-56", supplier: "LCSC", lowStockThreshold: 3, tags: ["mcu", "wifi", "project-beta"] },
  { id: "c-10", name: "2N2222 NPN", category: "Transistors", storageUnit: "Transistors", bin: "R01-C01", compartment: "P01", quantity: 75, package: "TO-92", supplier: "Digikey", lowStockThreshold: 15, tags: ["through-hole"] },
  { id: "c-11", name: "IRLZ44N MOSFET", category: "Transistors", storageUnit: "Transistors", bin: "R02-C01", compartment: "P01", quantity: 12, package: "TO-220", supplier: "Mouser", lowStockThreshold: 5, tags: ["power", "through-hole"] },
  { id: "c-12", name: "470 Resistor", category: "Resistors", storageUnit: "Resistors", bin: "R02-C01", compartment: "P01", quantity: 3, package: "0603", supplier: "Digikey", lowStockThreshold: 20, tags: ["smd"] },
  { id: "c-13", name: "LED Red 3mm", category: "LEDs", storageUnit: "Transistors", bin: "R03-C01", compartment: "P01", quantity: 5, package: "3mm", supplier: "LCSC", lowStockThreshold: 10, tags: ["indicator", "through-hole"] },
];

export const demoMovements: MovementRecord[] = [
  { id: "m-1", date: "2026-03-07T10:30:00", component: "10k Resistor", action: "used", quantity: -5, location: "R03-C02", user: "Demo User" },
  { id: "m-2", date: "2026-03-07T09:15:00", component: "STM32F103", action: "added", quantity: 10, location: "R01-C01", user: "Demo User" },
  { id: "m-3", date: "2026-03-06T16:45:00", component: "100nF Capacitor", action: "restocked", quantity: 50, location: "R01-C01", user: "Demo User" },
  { id: "m-4", date: "2026-03-06T14:20:00", component: "ESP32-S3", action: "used", quantity: -2, location: "R01-C02", user: "Demo User" },
  { id: "m-5", date: "2026-03-06T11:00:00", component: "4.7k Resistor", action: "added", quantity: 30, location: "R03-C03", user: "Demo User" },
  { id: "m-6", date: "2026-03-05T17:30:00", component: "2N2222 NPN", action: "used", quantity: -10, location: "R01-C01", user: "Demo User" },
  { id: "m-7", date: "2026-03-05T10:00:00", component: "10uF Capacitor", action: "restocked", quantity: 25, location: "R02-C01", user: "Demo User" },
  { id: "m-8", date: "2026-03-04T15:45:00", component: "1k Resistor", action: "moved", quantity: 0, location: "R01-C01", user: "Demo User" },
];

export const categories = [
  "Resistors",
  "Capacitors",
  "Inductors",
  "Transistors",
  "Microcontrollers",
  "LEDs",
  "Connectors",
  "ICs",
];
