"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import api from "@/lib/api";
import type { ImportBomResponse, BomHistoryPagedResponse } from "@/types/api";

export function useImportBom() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (file: File) => {
      const formData = new FormData();
      formData.append("file", file);
      const { data } = await api.post<ImportBomResponse>(
        "/api/bom/import",
        formData,
        { headers: { "Content-Type": "multipart/form-data" } }
      );
      return data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["bom-history"] });
      queryClient.invalidateQueries({ queryKey: ["components"] });
    },
  });
}

export function useBomHistory(params: { page?: number; pageSize?: number }) {
  return useQuery({
    queryKey: ["bom-history", params],
    queryFn: async () => {
      const { data } = await api.get<BomHistoryPagedResponse>(
        "/api/bom/history",
        { params }
      );
      return data;
    },
  });
}
