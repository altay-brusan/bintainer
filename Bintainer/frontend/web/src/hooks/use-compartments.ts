"use client";

import { useMutation, useQueryClient } from "@tanstack/react-query";
import api from "@/lib/api";

export function useAssignComponent() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({
      id,
      componentId,
      quantity,
    }: {
      id: string;
      componentId: string;
      quantity: number;
    }) => {
      const { data } = await api.put(`/api/compartments/${id}/component`, {
        componentId,
        quantity,
      });
      return data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["storage-unit"] });
      queryClient.invalidateQueries({ queryKey: ["components"] });
      queryClient.invalidateQueries({ queryKey: ["component"] });
    },
  });
}

export function useRemoveComponent() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (id: string) => {
      await api.delete(`/api/compartments/${id}/component`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["storage-unit"] });
      queryClient.invalidateQueries({ queryKey: ["components"] });
      queryClient.invalidateQueries({ queryKey: ["component"] });
    },
  });
}

export function useUpdateLabel() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ id, label }: { id: string; label: string }) => {
      const { data } = await api.put(`/api/compartments/${id}/label`, {
        label,
      });
      return data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["storage-unit"] });
    },
  });
}

export function useActivateCompartment() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (id: string) => {
      const { data } = await api.post(`/api/compartments/${id}/restore`);
      return data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["storage-unit"] });
    },
  });
}

export function useDeactivateCompartment() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (id: string) => {
      await api.delete(`/api/compartments/${id}`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["storage-unit"] });
    },
  });
}
