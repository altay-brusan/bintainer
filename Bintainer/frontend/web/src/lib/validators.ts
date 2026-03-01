import { z } from "zod";

export const loginSchema = z.object({
  email: z.string().email("Invalid email address"),
  password: z.string().min(1, "Password is required"),
});

export const registerSchema = z.object({
  email: z.string().email("Invalid email address"),
  password: z.string().min(6, "Password must be at least 6 characters"),
  firstName: z.string().min(1, "First name is required"),
  lastName: z.string().min(1, "Last name is required"),
});

export const createInventorySchema = z.object({
  name: z.string().min(1, "Name is required").max(200, "Name is too long"),
});

export const createStorageUnitSchema = z.object({
  name: z.string().min(1, "Name is required").max(200, "Name is too long"),
  columns: z.number().int().min(1, "At least 1 column"),
  rows: z.number().int().min(1, "At least 1 row"),
  compartmentCount: z.number().int().min(1, "At least 1 compartment"),
  inventoryId: z.string().uuid(),
});

export const updateStorageUnitSchema = z.object({
  name: z.string().min(1, "Name is required").max(200, "Name is too long"),
});

export type LoginInput = z.infer<typeof loginSchema>;
export type RegisterInput = z.infer<typeof registerSchema>;
export type CreateInventoryInput = z.infer<typeof createInventorySchema>;
export type CreateStorageUnitInput = z.infer<typeof createStorageUnitSchema>;
export type UpdateStorageUnitInput = z.infer<typeof updateStorageUnitSchema>;
