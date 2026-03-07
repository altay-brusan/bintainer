"use client";

import { useMutation, useQuery, useQueries, useQueryClient } from "@tanstack/react-query";
import api from "@/lib/api";
import type { StorageUnitSummary, StorageUnitDetail } from "@/types/api";
import type { CreateStorageUnitInput, UpdateStorageUnitInput } from "@/lib/validators";

export function useStorageUnits(inventoryId: string) {
  return useQuery({
    queryKey: ["storage-units", inventoryId],
    queryFn: async () => {
      const { data } = await api.get<StorageUnitSummary[]>(
        `/api/inventories/${inventoryId}/storage-units`
      );
      return data;
    },
    enabled: !!inventoryId,
  });
}

export function useAllStorageUnits(inventoryIds: string[]) {
  const queries = useQueries({
    queries: inventoryIds.map((id) => ({
      queryKey: ["storage-units", id],
      queryFn: async () => {
        const { data } = await api.get<StorageUnitSummary[]>(
          `/api/inventories/${id}/storage-units`
        );
        return data;
      },
      enabled: !!id,
    })),
  });

  const isLoading = queries.some((q) => q.isLoading);
  const data = queries
    .filter((q) => q.data)
    .flatMap((q) => q.data!);

  return { data: inventoryIds.length === 0 ? undefined : data, isLoading };
}

export function useStorageUnit(id: string) {
  return useQuery({
    queryKey: ["storage-unit", id],
    queryFn: async () => {
      const { data } = await api.get<StorageUnitDetail>(
        `/api/storage-units/${id}`
      );
      return data;
    },
    enabled: !!id,
  });
}

export function useCreateStorageUnit() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (input: CreateStorageUnitInput) => {
      const { data } = await api.post<string>("/api/storage-units", input);
      return data;
    },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ["storage-units", variables.inventoryId],
      });
    },
  });
}

export function useUpdateStorageUnit() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({
      id,
      ...input
    }: UpdateStorageUnitInput & { id: string }) => {
      const { data } = await api.put(`/api/storage-units/${id}`, input);
      return data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["storage-units"] });
      queryClient.invalidateQueries({ queryKey: ["storage-unit"] });
    },
  });
}

export function useDeleteStorageUnit() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (id: string) => {
      await api.delete(`/api/storage-units/${id}`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["storage-units"] });
    },
  });
}
