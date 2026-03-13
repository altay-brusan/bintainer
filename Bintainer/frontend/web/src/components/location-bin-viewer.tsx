"use client";

import { Suspense, lazy, useState } from "react";
import { useStorageUnit } from "@/hooks/use-storage-units";
import { Button } from "@/components/ui/button";
import { Grid3X3, Box } from "lucide-react";

const StorageUnit3DViewer = lazy(() =>
  import("@/components/storage-unit-3d-viewer").then((m) => ({
    default: m.StorageUnit3DViewer,
  }))
);

interface LocationBinViewerProps {
  storageUnitId: string;
  targetBinId: string;
}

export function LocationBinViewer({
  storageUnitId,
  targetBinId,
}: LocationBinViewerProps) {
  const { data: storageUnit, isLoading } = useStorageUnit(storageUnitId);
  const [view, setView] = useState<"2d" | "3d">("2d");

  if (isLoading) {
    return (
      <div className="flex h-[200px] items-center justify-center rounded-lg border bg-muted/30">
        <p className="text-sm text-muted-foreground">Loading bin layout...</p>
      </div>
    );
  }

  if (!storageUnit) return null;

  const targetBin = storageUnit.bins.find((b) => b.id === targetBinId);
  const selectedBin = targetBin
    ? { row: targetBin.row, col: targetBin.column }
    : null;

  const bins3d = storageUnit.bins
    .filter((b) => b.isActive)
    .map((b) => ({
      id: b.id,
      row: b.row,
      col: b.column,
      hasComponents: b.compartments.some((c) => c.componentId),
      label: b.compartments
        .filter((c) => c.isActive && c.label)
        .map((c) => c.label)
        .join(", "),
    }));

  return (
    <div className="space-y-2">
      <div className="flex items-center justify-between">
        <p className="text-xs font-medium text-muted-foreground">
          {storageUnit.name} — {storageUnit.rows}×{storageUnit.columns} grid
        </p>
        <div className="flex gap-1">
          <Button
            variant={view === "2d" ? "default" : "ghost"}
            size="icon"
            className="h-6 w-6"
            onClick={() => setView("2d")}
          >
            <Grid3X3 className="h-3.5 w-3.5" />
          </Button>
          <Button
            variant={view === "3d" ? "default" : "ghost"}
            size="icon"
            className="h-6 w-6"
            onClick={() => setView("3d")}
          >
            <Box className="h-3.5 w-3.5" />
          </Button>
        </div>
      </div>

      {view === "2d" ? (
        <div
          className="grid gap-1 rounded-lg border bg-muted/20 p-2"
          style={{
            gridTemplateColumns: `repeat(${storageUnit.columns}, 1fr)`,
          }}
        >
          {Array.from({ length: storageUnit.rows }, (_, row) =>
            Array.from({ length: storageUnit.columns }, (_, col) => {
              const bin = storageUnit.bins.find(
                (b) => b.row === row && b.column === col
              );
              const isTarget = bin?.id === targetBinId;
              const hasComponents =
                bin?.compartments.some((c) => c.componentId) ?? false;
              const isActive = bin?.isActive ?? false;

              return (
                <div
                  key={`${row}-${col}`}
                  className={`flex items-center justify-center rounded text-[10px] font-mono aspect-square transition-colors ${
                    isTarget
                      ? "bg-primary text-primary-foreground ring-2 ring-primary ring-offset-1"
                      : hasComponents && isActive
                        ? "bg-blue-100 dark:bg-blue-900/40 text-blue-700 dark:text-blue-300"
                        : isActive
                          ? "bg-muted text-muted-foreground"
                          : "bg-muted/30 text-muted-foreground/30"
                  }`}
                >
                  {row},{col}
                </div>
              );
            })
          )}
        </div>
      ) : (
        <Suspense
          fallback={
            <div className="flex h-[400px] max-w-[600px] mx-auto items-center justify-center rounded-lg border bg-muted/30">
              <p className="text-sm text-muted-foreground">
                Loading 3D viewer...
              </p>
            </div>
          }
        >
          <div className="max-w-[600px] mx-auto rounded-lg border overflow-hidden">
            <StorageUnit3DViewer
              rows={storageUnit.rows}
              columns={storageUnit.columns}
              bins={bins3d}
              selectedBin={selectedBin}
              onBinSelect={() => {}}
              occupiedColor="#60A5FA"
              occupiedHoverColor="#93C5FD"
            />
          </div>
        </Suspense>
      )}
    </div>
  );
}
