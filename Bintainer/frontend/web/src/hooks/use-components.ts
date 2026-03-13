"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import api from "@/lib/api";
import type {
  ComponentResponse,
  ComponentSummaryResponse,
  SearchComponentsPagedResponse,
} from "@/types/api";

export function useComponents(categoryId?: string) {
  return useQuery({
    queryKey: ["components", { categoryId }],
    queryFn: async () => {
      const params = categoryId ? { categoryId } : undefined;
      const { data } = await api.get<ComponentSummaryResponse[]>(
        "/api/components",
        { params }
      );
      return data;
    },
  });
}

export function useComponent(id: string) {
  return useQuery({
    queryKey: ["component", id],
    queryFn: async () => {
      const { data } = await api.get<ComponentResponse>(
        `/api/components/${id}`
      );
      return data;
    },
    enabled: !!id,
  });
}

export function useSearchComponents(params: {
  q?: string;
  categoryId?: string;
  provider?: string;
  tag?: string;
  footprintId?: string;
  page?: number;
  pageSize?: number;
}) {
  return useQuery({
    queryKey: ["components", "search", params],
    queryFn: async () => {
      const { data } = await api.get<SearchComponentsPagedResponse>(
        "/api/components/search",
        { params }
      );
      return data;
    },
  });
}

export function useCreateComponent() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (
      input: Omit<ComponentResponse, "id" | "locations" | "categoryName" | "footprintName">
    ) => {
      const { data } = await api.post<string>("/api/components", input);
      return data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["components"] });
    },
  });
}

export function useUpdateComponent() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({
      id,
      ...input
    }: { id: string } & Partial<
      Omit<ComponentResponse, "id" | "locations" | "categoryName" | "footprintName">
    >) => {
      const { data } = await api.put(`/api/components/${id}`, input);
      return data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["components"] });
      queryClient.invalidateQueries({ queryKey: ["component"] });
    },
  });
}

export function useDeleteComponent() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (id: string) => {
      await api.delete(`/api/components/${id}`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["components"] });
      queryClient.invalidateQueries({ queryKey: ["component"] });
    },
  });
}

export function useUploadComponentImage() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ id, file }: { id: string; file: File }) => {
      const formData = new FormData();
      formData.append("file", file);
      const { data } = await api.post(
        `/api/components/${id}/image`,
        formData,
        { headers: { "Content-Type": "multipart/form-data" } }
      );
      return data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["components"] });
      queryClient.invalidateQueries({ queryKey: ["component"] });
    },
  });
}
