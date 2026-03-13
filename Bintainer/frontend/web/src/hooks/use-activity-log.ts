"use client";

import { useQuery } from "@tanstack/react-query";
import api from "@/lib/api";
import type { ActivityLogPagedResponse } from "@/types/api";

export function useActivityLog(params: {
  action?: string;
  entityType?: string;
  entityId?: string;
  page?: number;
  pageSize?: number;
}) {
  return useQuery({
    queryKey: ["activity-log", params],
    queryFn: async () => {
      const { data } = await api.get<ActivityLogPagedResponse>(
        "/api/activity-log",
        { params }
      );
      return data;
    },
  });
}
