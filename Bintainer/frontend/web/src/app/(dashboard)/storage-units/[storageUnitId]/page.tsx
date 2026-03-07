"use client";

import { use, useState, useMemo, useCallback, lazy, Suspense } from "react";
import { ArrowLeft, Settings2, Grid3x3, Box, X, Plus, Minus } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Badge } from "@/components/ui/badge";
import { cn } from "@/lib/utils";
import { demoStorageUnits, demoComponents } from "@/lib/demo-data";
import Link from "next/link";

const StorageUnit3DViewer = lazy(() =>
  import("@/components/storage-unit-3d-viewer").then((m) => ({
    default: m.StorageUnit3DViewer,
  }))
);

interface PageProps {
  params: Promise<{ storageUnitId: string }>;
}

export default function StorageUnitEditorPage({ params }: PageProps) {
  const { storageUnitId } = use(params);
  const storageUnit = demoStorageUnits.find((su) => su.id === storageUnitId);
  const [selectedBin, setSelectedBin] = useState<{ row: number; col: number } | null>(null);
  const [viewMode, setViewMode] = useState<"grid" | "3d">("grid");
  const [editRows, setEditRows] = useState(storageUnit?.rows ?? 1);
  const [editColumns, setEditColumns] = useState(storageUnit?.columns ?? 1);
  const [deletedBins, setDeletedBins] = useState<Set<string>>(new Set());
  const [removedBins, setRemovedBins] = useState<Set<string>>(new Set());
  // Map of "row-col" → compartment count (default 1 for every bin, max 5)
  const [binCompartments, setBinCompartments] = useState<Map<string, number>>(new Map());
  const [selectedCompartment, setSelectedCompartment] = useState<number | null>(null);

  const componentsInUnit = useMemo(
    () => demoComponents.filter((c) => c.storageUnit === storageUnit?.name),
    [storageUnit]
  );

  const selectedBinComponents = useMemo(() => {
    if (!selectedBin || !storageUnit) return [];
    const binLabel = `R${String(selectedBin.row + 1).padStart(2, "0")}-C${String(selectedBin.col + 1).padStart(2, "0")}`;
    return componentsInUnit.filter((c) => c.bin === binLabel);
  }, [selectedBin, componentsInUnit, storageUnit]);

  const selectBin = useCallback((bin: { row: number; col: number } | null) => {
    setSelectedBin(bin);
    setSelectedCompartment(null);
  }, []);

  const selectedBinKey = selectedBin ? `${selectedBin.row}-${selectedBin.col}` : null;
  const selectedBinCompartmentCount = selectedBinKey ? (binCompartments.get(selectedBinKey) ?? 1) : 1;

  const setCompartmentCount = useCallback((key: string, count: number) => {
    setBinCompartments((prev) => {
      const next = new Map(prev);
      if (count <= 1) {
        next.delete(key);
      } else {
        next.set(key, count);
      }
      return next;
    });
  }, []);

  const toggleDeleteBin = useCallback((row: number, col: number) => {
    setDeletedBins((prev) => {
      const key = `${row}-${col}`;
      const next = new Set(prev);
      if (next.has(key)) {
        next.delete(key);
      } else {
        next.add(key);
      }
      return next;
    });
  }, []);

  const handleSave = useCallback(() => {
    // Move marked bins into permanently removed set
    setRemovedBins((prev) => {
      const next = new Set(prev);
      for (const key of deletedBins) {
        next.add(key);
      }
      return next;
    });
    setDeletedBins(new Set());
    selectBin(null);
  }, [deletedBins]);

  if (!storageUnit) {
    return (
      <div className="space-y-4">
        <Link href="/storage-units" className="inline-flex items-center gap-2 text-sm text-muted-foreground hover:text-foreground">
          <ArrowLeft className="h-4 w-4" /> Back to Storage Units
        </Link>
        <div className="rounded-xl border border-dashed bg-card p-12 text-center">
          <p className="text-muted-foreground">Storage unit not found</p>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-4">
          <Link href="/storage-units" className="rounded-lg p-2 hover:bg-muted">
            <ArrowLeft className="h-5 w-5" />
          </Link>
          <div>
            <h1 className="text-2xl font-bold">Edit Storage Unit</h1>
            <p className="text-muted-foreground">{storageUnit.name}</p>
          </div>
        </div>
        <Button className="gap-2" onClick={handleSave}>
          <Settings2 className="h-4 w-4" />
          Save Storage Unit
        </Button>
      </div>

      <div className="grid gap-6 lg:grid-cols-[1fr_320px]">
        {/* Left: Grid + Config */}
        <div className="space-y-6">
          {/* Unit Config */}
          <div className="rounded-xl border bg-card p-5 shadow-sm">
            <h3 className="mb-4 font-semibold">Configuration</h3>
            <div className="grid grid-cols-3 gap-4">
              <div>
                <Label>Name</Label>
                <Input defaultValue={storageUnit.name} className="mt-1" />
              </div>
              <div>
                <Label>Rows</Label>
                <Input
                  type="number"
                  min={1}
                  max={50}
                  value={editRows}
                  onChange={(e) => setEditRows(Math.max(1, parseInt(e.target.value) || 1))}
                  className="mt-1"
                />
              </div>
              <div>
                <Label>Columns</Label>
                <Input
                  type="number"
                  min={1}
                  max={50}
                  value={editColumns}
                  onChange={(e) => setEditColumns(Math.max(1, parseInt(e.target.value) || 1))}
                  className="mt-1"
                />
              </div>
            </div>
            <p className="mt-3 text-sm text-muted-foreground">
              Grid size: {editRows * editColumns} bins
              {deletedBins.size > 0 && (
                <span className="text-red-500"> ({deletedBins.size} marked for removal)</span>
              )}
              {" "}&middot; {componentsInUnit.length} components stored &middot;{" "}
              <Link href="/find" className="text-primary hover:underline">
                View Components
              </Link>
            </p>
          </div>

          {/* Bin View */}
          <div className="rounded-xl border bg-card p-5 shadow-sm">
            <div className="mb-4 flex items-center justify-between">
              <h3 className="font-semibold">Bin Grid</h3>
              <div className="flex rounded-lg border p-0.5">
                <Button
                  variant={viewMode === "3d" ? "default" : "ghost"}
                  size="sm"
                  className="gap-1.5 h-8 px-3"
                  onClick={() => setViewMode("3d")}
                >
                  <Box className="h-3.5 w-3.5" />
                  3D
                </Button>
                <Button
                  variant={viewMode === "grid" ? "default" : "ghost"}
                  size="sm"
                  className="gap-1.5 h-8 px-3"
                  onClick={() => setViewMode("grid")}
                >
                  <Grid3x3 className="h-3.5 w-3.5" />
                  Grid
                </Button>
              </div>
            </div>

            {viewMode === "3d" && (
              <Suspense
                fallback={
                  <div className="flex h-[500px] items-center justify-center rounded-xl border bg-slate-900">
                    <p className="text-sm text-slate-400">Loading 3D viewer...</p>
                  </div>
                }
              >
                <StorageUnit3DViewer
                  rows={editRows}
                  columns={editColumns}
                  bins={Array.from({ length: editRows }).flatMap((_, r) =>
                    Array.from({ length: editColumns }).map((_, c) => {
                      const binLabel = `R${String(r + 1).padStart(2, "0")}-C${String(c + 1).padStart(2, "0")}`;
                      return {
                        id: `${r}-${c}`,
                        row: r,
                        col: c,
                        hasComponents: componentsInUnit.some((comp) => comp.bin === binLabel),
                      };
                    })
                  )}
                  selectedBin={selectedBin}
                  onBinSelect={(row, col) => selectBin({ row, col })}
                  deletedBins={deletedBins}
                  removedBins={removedBins}
                  onBinDoubleClick={toggleDeleteBin}
                  onBinRestore={(row, col) => setRemovedBins((prev) => { const next = new Set(prev); next.delete(`${row}-${col}`); return next; })}
                />
              </Suspense>
            )}

            {viewMode === "grid" && (
              <>


            {/* Column headers */}
            <div className="flex gap-1.5 mb-1.5">
              <div className="w-10" />
              {Array.from({ length: editColumns }).map((_, c) => (
                <div
                  key={c}
                  className="flex h-6 w-10 items-center justify-center text-xs font-medium text-muted-foreground"
                >
                  {String(c + 1).padStart(2, "0")}
                </div>
              ))}
            </div>

            {/* Grid rows */}
            {Array.from({ length: editRows }).map((_, r) => (
              <div key={r} className="flex gap-1.5 mb-1.5">
                <div className="flex w-10 items-center justify-center text-xs font-medium text-muted-foreground">
                  {String(r + 1).padStart(2, "0")}
                </div>
                {Array.from({ length: editColumns }).map((_, c) => {
                  const key = `${r}-${c}`;
                  const isRemoved = removedBins.has(key);
                  if (isRemoved) {
                    return (
                      <button
                        key={c}
                        onDoubleClick={() => setRemovedBins((prev) => { const next = new Set(prev); next.delete(key); return next; })}
                        className="h-10 w-10 rounded-md border border-dashed border-muted-foreground/20 hover:border-primary/50 hover:bg-primary/5 transition-colors"
                      />
                    );
                  }
                  const binLabel = `R${String(r + 1).padStart(2, "0")}-C${String(c + 1).padStart(2, "0")}`;
                  const hasComponents = componentsInUnit.some((comp) => comp.bin === binLabel);
                  const isSelected = selectedBin?.row === r && selectedBin?.col === c;
                  const isDeleted = deletedBins.has(key);

                  return (
                    <button
                      key={c}
                      onClick={() => selectBin({ row: r, col: c })}
                      onDoubleClick={() => toggleDeleteBin(r, c)}
                      className={cn(
                        "relative flex h-10 w-10 items-center justify-center rounded-md text-xs font-medium transition-all",
                        isDeleted
                          ? "bg-red-100 dark:bg-red-950 text-red-400 border border-red-300 dark:border-red-800 opacity-60"
                          : isSelected
                            ? "bg-primary text-primary-foreground ring-2 ring-primary/50 scale-105"
                            : hasComponents
                              ? "bg-blue-400 dark:bg-blue-500 text-white hover:bg-blue-500 dark:hover:bg-blue-400"
                              : "bg-muted text-muted-foreground hover:bg-muted/80"
                      )}
                    >
                      {isDeleted ? (
                        <X className="h-5 w-5 text-red-500" />
                      ) : (
                        String(c + 1).padStart(2, "0")
                      )}
                    </button>
                  );
                })}
              </div>
            ))}

            <div className="mt-4 flex items-center gap-4 text-xs text-muted-foreground">
              <div className="flex items-center gap-1.5">
                <div className="h-3 w-3 rounded-sm bg-primary" /> Selected
              </div>
              <div className="flex items-center gap-1.5">
                <div className="h-3 w-3 rounded-sm bg-blue-400" /> Has components
              </div>
              <div className="flex items-center gap-1.5">
                <div className="h-3 w-3 rounded-sm bg-muted" /> Empty
              </div>
              <div className="flex items-center gap-1.5">
                <div className="h-3 w-3 rounded-sm border border-red-400 bg-red-100 dark:bg-red-950" /> Marked for removal
              </div>
            </div>

              </>
            )}
          </div>
        </div>

        {/* Right: Bin Details */}
        <div className="space-y-4">
          <div className="rounded-xl border bg-card p-5 shadow-sm">
            <h3 className="mb-1 font-semibold">Bin Options</h3>
            {selectedBin ? (
              <div className="space-y-4">
                <div>
                  <p className="text-sm text-muted-foreground">Selected Bin</p>
                  <p className="font-mono text-lg font-bold">
                    R{String(selectedBin.row + 1).padStart(2, "0")}-C{String(selectedBin.col + 1).padStart(2, "0")}
                  </p>
                </div>

                <div>
                  <div className="flex items-center justify-between mb-2">
                    <h4 className="text-sm font-medium">Compartments</h4>
                    <div className="flex items-center gap-1.5">
                      <Button
                        variant="outline"
                        size="icon"
                        className="h-7 w-7"
                        disabled={selectedBinCompartmentCount <= 1}
                        onClick={() => selectedBinKey && setCompartmentCount(selectedBinKey, selectedBinCompartmentCount - 1)}
                      >
                        <Minus className="h-3.5 w-3.5" />
                      </Button>
                      <span className="w-6 text-center text-sm font-medium">{selectedBinCompartmentCount}</span>
                      <Button
                        variant="outline"
                        size="icon"
                        className="h-7 w-7"
                        disabled={selectedBinCompartmentCount >= 5}
                        onClick={() => selectedBinKey && setCompartmentCount(selectedBinKey, selectedBinCompartmentCount + 1)}
                      >
                        <Plus className="h-3.5 w-3.5" />
                      </Button>
                    </div>
                  </div>

                  <div className="space-y-2">
                    {Array.from({ length: selectedBinCompartmentCount }).map((_, i) => {
                      const compartmentLabel = `C${i + 1}`;
                      const comp = selectedBinComponents.find((c) => c.compartment === compartmentLabel);
                      const isActive = selectedCompartment === i;
                      return (
                        <button
                          key={i}
                          type="button"
                          onClick={() => setSelectedCompartment(isActive ? null : i)}
                          className={cn(
                            "w-full rounded-lg border bg-background p-3 text-left transition-all",
                            isActive
                              ? "ring-2 ring-primary/50 border-primary"
                              : "hover:border-muted-foreground/30"
                          )}
                        >
                          <div className="flex items-center justify-between">
                            <p className="text-sm font-medium">{comp ? comp.name : "Empty"}</p>
                            <Badge variant={isActive ? "default" : "secondary"} className="text-xs">
                              {compartmentLabel}
                            </Badge>
                          </div>
                          {comp ? (
                            <p className="mt-1 text-sm text-muted-foreground">
                              Qty: {comp.quantity}
                            </p>
                          ) : (
                            <p className="mt-1 text-sm text-muted-foreground italic">
                              No component assigned
                            </p>
                          )}
                        </button>
                      );
                    })}
                  </div>
                </div>

                <Button variant="outline" className="w-full" size="sm" disabled={selectedCompartment === null}>
                  <Plus className="h-3.5 w-3.5 mr-1.5" />
                  {selectedCompartment !== null ? `Add Component to C${selectedCompartment + 1}` : "Add Component"}
                </Button>
              </div>
            ) : (
              <p className="mt-2 text-sm text-muted-foreground">
                Click a bin in the grid to view its details
              </p>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
