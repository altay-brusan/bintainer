"use client";

import { cn } from "@/lib/utils";
import Link from "next/link";

interface StorageUnitDetailCardProps {
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
    header: "bg-blue-500 dark:bg-blue-600",
    cell: "bg-blue-400 dark:bg-blue-500",
    cellEmpty: "bg-blue-200 dark:bg-blue-800",
    text: "text-blue-700 dark:text-blue-300",
  },
  amber: {
    header: "bg-amber-500 dark:bg-amber-600",
    cell: "bg-amber-400 dark:bg-amber-500",
    cellEmpty: "bg-amber-200 dark:bg-amber-800",
    text: "text-amber-700 dark:text-amber-300",
  },
  emerald: {
    header: "bg-emerald-500 dark:bg-emerald-600",
    cell: "bg-emerald-400 dark:bg-emerald-500",
    cellEmpty: "bg-emerald-200 dark:bg-emerald-800",
    text: "text-emerald-700 dark:text-emerald-300",
  },
  red: {
    header: "bg-red-500 dark:bg-red-600",
    cell: "bg-red-400 dark:bg-red-500",
    cellEmpty: "bg-red-200 dark:bg-red-800",
    text: "text-red-700 dark:text-red-300",
  },
  purple: {
    header: "bg-purple-500 dark:bg-purple-600",
    cell: "bg-purple-400 dark:bg-purple-500",
    cellEmpty: "bg-purple-200 dark:bg-purple-800",
    text: "text-purple-700 dark:text-purple-300",
  },
};

export function StorageUnitDetailCard({
  id,
  name,
  rows,
  columns,
  bins,
  components,
  colorScheme = "blue",
}: StorageUnitDetailCardProps) {
  const colors = colorMap[colorScheme];
  const displayRows = Math.min(rows, 5);
  const displayCols = Math.min(columns, 7);

  return (
    <Link
      href={`/storage-units/${id}`}
      className="block rounded-xl border bg-card shadow-sm transition-shadow hover:shadow-md overflow-hidden"
    >
      <div className={cn("px-4 py-2.5 text-white", colors.header)}>
        <div className="flex items-center justify-between">
          <h4 className="font-semibold">{name}</h4>
          <span className="text-sm opacity-90">{components}</span>
        </div>
      </div>
      <div className="p-4">
        <div className="mb-3 flex gap-4 text-xs text-muted-foreground">
          <span>{bins} Bins</span>
          <span>{components} Components</span>
          <span>{rows}r</span>
        </div>
        <div className="flex flex-col gap-1">
          {Array.from({ length: displayRows }).map((_, r) => (
            <div key={r} className="flex gap-1">
              <span className="mr-1 w-4 text-right text-[10px] text-muted-foreground leading-4">
                {String(r + 1).padStart(2, "0")}
              </span>
              {Array.from({ length: displayCols }).map((_, c) => {
                const filled = (r * displayCols + c) < bins;
                return (
                  <div
                    key={c}
                    className={cn(
                      "flex h-4 w-4 items-center justify-center rounded-sm text-[8px] font-medium text-white",
                      filled ? colors.cell : colors.cellEmpty
                    )}
                  >
                    {String(c + 1).padStart(2, "0")}
                  </div>
                );
              })}
            </div>
          ))}
        </div>
      </div>
    </Link>
  );
}
