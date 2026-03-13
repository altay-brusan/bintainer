"use client";

import { useMutation, useQueryClient } from "@tanstack/react-query";
import api from "@/lib/api";

export function useAdjustQuantity() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({
      id,
      compartmentId,
      action,
      quantity,
      notes,
    }: {
      id: string;
      compartmentId: string;
      action: string;
      quantity: number;
      notes?: string;
    }) => {
      const { data } = await api.post(`/api/components/${id}/quantity`, {
        compartmentId,
        action,
        quantity,
        notes,
      });
      return data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["components"] });
      queryClient.invalidateQueries({ queryKey: ["component"] });
      queryClient.invalidateQueries({ queryKey: ["storage-unit"] });
      queryClient.invalidateQueries({ queryKey: ["movements"] });
    },
  });
}

export function useMoveComponent() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({
      id,
      sourceCompartmentId,
      destinationCompartmentId,
      quantity,
    }: {
      id: string;
      sourceCompartmentId: string;
      destinationCompartmentId: string;
      quantity: number;
    }) => {
      const { data } = await api.post(`/api/components/${id}/move`, {
        sourceCompartmentId,
        destinationCompartmentId,
        quantity,
      });
      return data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["components"] });
      queryClient.invalidateQueries({ queryKey: ["component"] });
      queryClient.invalidateQueries({ queryKey: ["storage-unit"] });
      queryClient.invalidateQueries({ queryKey: ["movements"] });
    },
  });
}
