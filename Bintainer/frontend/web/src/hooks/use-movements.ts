"use client";

import { useQuery } from "@tanstack/react-query";
import api from "@/lib/api";
import type { MovementsPagedResponse } from "@/types/api";

export function useMovements(params: {
  action?: string;
  componentId?: string;
  q?: string;
  page?: number;
  pageSize?: number;
}) {
  return useQuery({
    queryKey: ["movements", params],
    queryFn: async () => {
      const { data } = await api.get<MovementsPagedResponse>(
        "/api/movements",
        { params }
      );
      return data;
    },
  });
}
