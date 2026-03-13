"use client";

import { useQuery } from "@tanstack/react-query";
import api from "@/lib/api";
import type {
  SummaryResponse,
  TopComponentResponse,
  LowStockResponse,
  StorageUtilizationResponse,
  MovementTimelineResponse,
  SupplierDistributionResponse,
  CategoryDistributionResponse,
} from "@/types/api";

export function useSummary() {
  return useQuery({
    queryKey: ["reports", "summary"],
    queryFn: async () => {
      const { data } = await api.get<SummaryResponse>(
        "/api/reports/summary"
      );
      return data;
    },
  });
}

export function useTopComponents(sortBy?: string, limit?: number) {
  return useQuery({
    queryKey: ["reports", "top-components", { sortBy, limit }],
    queryFn: async () => {
      const { data } = await api.get<TopComponentResponse[]>(
        "/api/reports/top-components",
        { params: { sortBy, limit } }
      );
      return data;
    },
  });
}

export function useLowStock() {
  return useQuery({
    queryKey: ["reports", "low-stock"],
    queryFn: async () => {
      const { data } = await api.get<LowStockResponse[]>(
        "/api/reports/low-stock"
      );
      return data;
    },
  });
}

export function useStorageUtilization() {
  return useQuery({
    queryKey: ["reports", "storage-utilization"],
    queryFn: async () => {
      const { data } = await api.get<StorageUtilizationResponse[]>(
        "/api/reports/storage-utilization"
      );
      return data;
    },
  });
}

export function useMovementTimeline(days?: number) {
  return useQuery({
    queryKey: ["reports", "movement-timeline", { days }],
    queryFn: async () => {
      const { data } = await api.get<MovementTimelineResponse[]>(
        "/api/reports/movement-timeline",
        { params: { days } }
      );
      return data;
    },
  });
}

export function useSupplierDistribution() {
  return useQuery({
    queryKey: ["reports", "supplier-distribution"],
    queryFn: async () => {
      const { data } = await api.get<SupplierDistributionResponse[]>(
        "/api/reports/supplier-distribution"
      );
      return data;
    },
  });
}

export function useCategoryDistribution() {
  return useQuery({
    queryKey: ["reports", "category-distribution"],
    queryFn: async () => {
      const { data } = await api.get<CategoryDistributionResponse[]>(
        "/api/reports/category-distribution"
      );
      return data;
    },
  });
}
