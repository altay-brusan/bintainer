"use client";

import { useMutation, useQueryClient } from "@tanstack/react-query";
import api from "@/lib/api";

export function useActivateBin() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (id: string) => {
      const { data } = await api.post(`/api/bins/${id}/restore`);
      return data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["storage-unit"] });
    },
  });
}

export function useDeactivateBin() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (id: string) => {
      await api.delete(`/api/bins/${id}`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["storage-unit"] });
    },
  });
}
