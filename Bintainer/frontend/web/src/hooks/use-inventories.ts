"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import api from "@/lib/api";
import type { Inventory } from "@/types/api";

export function useInventories() {
  return useQuery({
    queryKey: ["inventories"],
    queryFn: async () => {
      const { data } = await api.get<Inventory[]>("/api/inventories");
      return data;
    },
  });
}

export function useCreateInventory() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (input: { name: string }) => {
      const { data } = await api.post<string>("/api/inventories", input);
      return data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["inventories"] });
    },
  });
}
