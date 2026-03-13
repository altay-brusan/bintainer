"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import api from "@/lib/api";
import type { FootprintResponse } from "@/types/api";

export function useFootprints() {
  return useQuery({
    queryKey: ["footprints"],
    queryFn: async () => {
      const { data } = await api.get<FootprintResponse[]>("/api/footprints");
      return data;
    },
  });
}

export function useCreateFootprint() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (input: { name: string }) => {
      const { data } = await api.post<string>("/api/footprints", input);
      return data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["footprints"] });
    },
  });
}

export function useUpdateFootprint() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ id, ...input }: { id: string; name: string }) => {
      const { data } = await api.put(`/api/footprints/${id}`, input);
      return data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["footprints"] });
    },
  });
}

export function useDeleteFootprint() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (id: string) => {
      await api.delete(`/api/footprints/${id}`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["footprints"] });
    },
  });
}
