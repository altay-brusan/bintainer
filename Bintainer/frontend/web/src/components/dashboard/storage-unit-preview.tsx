"use client";

import { cn } from "@/lib/utils";
import Link from "next/link";

interface StorageUnitPreviewProps {
  id: string;
  name: string;
  rows: number;
  columns: number;
  bins: number;
  components: number;
  colorScheme?: "blue" | "amber" | "emerald" | "red" | "purple";
}

const colorMap = {
  blue: {
    bg: "bg-blue-50 dark:bg-blue-950/30",
    border: "border-blue-200 dark:border-blue-800",
    cell: "bg-blue-400 dark:bg-blue-500",
    cellEmpty: "bg-blue-100 dark:bg-blue-900/50",
    text: "text-blue-700 dark:text-blue-300",
    badge: "bg-blue-100 dark:bg-blue-900/50 text-blue-700 dark:text-blue-300",
  },
  amber: {
    bg: "bg-amber-50 dark:bg-amber-950/30",
    border: "border-amber-200 dark:border-amber-800",
    cell: "bg-amber-400 dark:bg-amber-500",
    cellEmpty: "bg-amber-100 dark:bg-amber-900/50",
    text: "text-amber-700 dark:text-amber-300",
    badge: "bg-amber-100 dark:bg-amber-900/50 text-amber-700 dark:text-amber-300",
  },
  emerald: {
    bg: "bg-emerald-50 dark:bg-emerald-950/30",
    border: "border-emerald-200 dark:border-emerald-800",
    cell: "bg-emerald-400 dark:bg-emerald-500",
    cellEmpty: "bg-emerald-100 dark:bg-emerald-900/50",
    text: "text-emerald-700 dark:text-emerald-300",
    badge: "bg-emerald-100 dark:bg-emerald-900/50 text-emerald-700 dark:text-emerald-300",
  },
  red: {
    bg: "bg-red-50 dark:bg-red-950/30",
    border: "border-red-200 dark:border-red-800",
    cell: "bg-red-400 dark:bg-red-500",
    cellEmpty: "bg-red-100 dark:bg-red-900/50",
    text: "text-red-700 dark:text-red-300",
    badge: "bg-red-100 dark:bg-red-900/50 text-red-700 dark:text-red-300",
  },
  purple: {
    bg: "bg-purple-50 dark:bg-purple-950/30",
    border: "border-purple-200 dark:border-purple-800",
    cell: "bg-purple-400 dark:bg-purple-500",
    cellEmpty: "bg-purple-100 dark:bg-purple-900/50",
    text: "text-purple-700 dark:text-purple-300",
    badge: "bg-purple-100 dark:bg-purple-900/50 text-purple-700 dark:text-purple-300",
  },
};

export function StorageUnitPreview({
  id,
  name,
  rows,
  columns,
  bins,
  components,
  colorScheme = "blue",
}: StorageUnitPreviewProps) {
  const colors = colorMap[colorScheme];
  const previewRows = Math.min(rows, 4);
  const previewCols = Math.min(columns, 6);

  return (
    <Link
      href={`/storage-units/${id}`}
      className={cn(
        "block rounded-xl border p-4 transition-shadow hover:shadow-md",
        colors.bg,
        colors.border
      )}
    >
      <div className="mb-3 flex items-center justify-between">
        <h3 className={cn("font-semibold", colors.text)}>{name}</h3>
      </div>
      <div className="mb-3 flex gap-3 text-xs">
        <span className={cn("rounded-md px-2 py-1 font-medium", colors.badge)}>
          {bins} Bins
        </span>
        <span className={cn("rounded-md px-2 py-1 font-medium", colors.badge)}>
          {components} Parts
        </span>
      </div>
      <div className="flex flex-col gap-1">
        {Array.from({ length: previewRows }).map((_, r) => (
          <div key={r} className="flex gap-1">
            {Array.from({ length: previewCols }).map((_, c) => {
              const filled = (r * previewCols + c) < bins;
              return (
                <div
                  key={c}
                  className={cn(
                    "h-3.5 w-3.5 rounded-sm",
                    filled ? colors.cell : colors.cellEmpty
                  )}
                />
              );
            })}
          </div>
        ))}
      </div>
    </Link>
  );
}
