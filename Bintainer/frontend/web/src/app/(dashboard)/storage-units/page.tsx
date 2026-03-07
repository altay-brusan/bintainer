"use client";

import { Plus, Archive, Grid3x3, Layers } from "lucide-react";
import { Button } from "@/components/ui/button";
import { demoStorageUnits } from "@/lib/demo-data";
import Link from "next/link";
import { cn } from "@/lib/utils";

const colorSchemes = [
  { bg: "bg-blue-50 dark:bg-blue-950/30", border: "border-blue-200 dark:border-blue-800", header: "text-blue-700 dark:text-blue-300", icon: "bg-blue-100 dark:bg-blue-900/50 text-blue-600 dark:text-blue-400" },
  { bg: "bg-amber-50 dark:bg-amber-950/30", border: "border-amber-200 dark:border-amber-800", header: "text-amber-700 dark:text-amber-300", icon: "bg-amber-100 dark:bg-amber-900/50 text-amber-600 dark:text-amber-400" },
  { bg: "bg-emerald-50 dark:bg-emerald-950/30", border: "border-emerald-200 dark:border-emerald-800", header: "text-emerald-700 dark:text-emerald-300", icon: "bg-emerald-100 dark:bg-emerald-900/50 text-emerald-600 dark:text-emerald-400" },
  { bg: "bg-red-50 dark:bg-red-950/30", border: "border-red-200 dark:border-red-800", header: "text-red-700 dark:text-red-300", icon: "bg-red-100 dark:bg-red-900/50 text-red-600 dark:text-red-400" },
  { bg: "bg-purple-50 dark:bg-purple-950/30", border: "border-purple-200 dark:border-purple-800", header: "text-purple-700 dark:text-purple-300", icon: "bg-purple-100 dark:bg-purple-900/50 text-purple-600 dark:text-purple-400" },
];

export default function StorageUnitsPage() {
  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold">Storage Units</h1>
          <p className="text-muted-foreground">
            Manage your storage cabinets ({demoStorageUnits.length} units)
          </p>
        </div>
        <Button className="gap-2">
          <Plus className="h-4 w-4" />
          Create Storage Unit
        </Button>
      </div>

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {demoStorageUnits.map((su, i) => {
          const colors = colorSchemes[i % colorSchemes.length];
          const totalBins = su.rows * su.columns;
          return (
            <Link
              key={su.id}
              href={`/storage-units/${su.id}`}
              className={cn(
                "rounded-xl border p-5 transition-shadow hover:shadow-md",
                colors.bg,
                colors.border
              )}
            >
              <h3 className={cn("text-lg font-semibold mb-3", colors.header)}>
                {su.name}
              </h3>
              <div className="space-y-2">
                <div className="flex items-center gap-2 text-sm text-muted-foreground">
                  <div className={cn("flex h-7 w-7 items-center justify-center rounded-md", colors.icon)}>
                    <Grid3x3 className="h-3.5 w-3.5" />
                  </div>
                  <span>{totalBins} bins</span>
                  <span className="text-muted-foreground/50">|</span>
                  <span>{su.rows} rows x {su.columns} cols</span>
                </div>
                <div className="flex items-center gap-2 text-sm text-muted-foreground">
                  <div className={cn("flex h-7 w-7 items-center justify-center rounded-md", colors.icon)}>
                    <Layers className="h-3.5 w-3.5" />
                  </div>
                  <span>{su.compartmentCount} components</span>
                </div>
              </div>
            </Link>
          );
        })}
      </div>
    </div>
  );
}
