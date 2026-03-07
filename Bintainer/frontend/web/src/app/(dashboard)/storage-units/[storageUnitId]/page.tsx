"use client";

import { use, useState, useMemo } from "react";
import { ArrowLeft, Settings2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Badge } from "@/components/ui/badge";
import { cn } from "@/lib/utils";
import { demoStorageUnits, demoComponents } from "@/lib/demo-data";
import Link from "next/link";

interface PageProps {
  params: Promise<{ storageUnitId: string }>;
}

export default function StorageUnitEditorPage({ params }: PageProps) {
  const { storageUnitId } = use(params);
  const storageUnit = demoStorageUnits.find((su) => su.id === storageUnitId);
  const [selectedBin, setSelectedBin] = useState<{ row: number; col: number } | null>(null);

  const componentsInUnit = useMemo(
    () => demoComponents.filter((c) => c.storageUnit === storageUnit?.name),
    [storageUnit]
  );

  const selectedBinComponents = useMemo(() => {
    if (!selectedBin || !storageUnit) return [];
    const binLabel = `R${String(selectedBin.row + 1).padStart(2, "0")}-C${String(selectedBin.col + 1).padStart(2, "0")}`;
    return componentsInUnit.filter((c) => c.bin === binLabel);
  }, [selectedBin, componentsInUnit, storageUnit]);

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
        <Button className="gap-2">
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
                <Input type="number" defaultValue={storageUnit.rows} className="mt-1" />
              </div>
              <div>
                <Label>Columns</Label>
                <Input type="number" defaultValue={storageUnit.columns} className="mt-1" />
              </div>
            </div>
            <p className="mt-3 text-sm text-muted-foreground">
              Grid size: {storageUnit.rows * storageUnit.columns} bins &middot;{" "}
              {componentsInUnit.length} components stored &middot;{" "}
              <Link href="/find" className="text-primary hover:underline">
                View Components
              </Link>
            </p>
          </div>

          {/* Bin Grid */}
          <div className="rounded-xl border bg-card p-5 shadow-sm">
            <h3 className="mb-4 font-semibold">Bin Grid</h3>

            {/* Column headers */}
            <div className="flex gap-1.5 mb-1.5">
              <div className="w-10" />
              {Array.from({ length: storageUnit.columns }).map((_, c) => (
                <div
                  key={c}
                  className="flex h-6 w-10 items-center justify-center text-xs font-medium text-muted-foreground"
                >
                  {String(c + 1).padStart(2, "0")}
                </div>
              ))}
            </div>

            {/* Grid rows */}
            {Array.from({ length: storageUnit.rows }).map((_, r) => (
              <div key={r} className="flex gap-1.5 mb-1.5">
                <div className="flex w-10 items-center justify-center text-xs font-medium text-muted-foreground">
                  {String(r + 1).padStart(2, "0")}
                </div>
                {Array.from({ length: storageUnit.columns }).map((_, c) => {
                  const binLabel = `R${String(r + 1).padStart(2, "0")}-C${String(c + 1).padStart(2, "0")}`;
                  const hasComponents = componentsInUnit.some((comp) => comp.bin === binLabel);
                  const isSelected = selectedBin?.row === r && selectedBin?.col === c;

                  return (
                    <button
                      key={c}
                      onClick={() => setSelectedBin({ row: r, col: c })}
                      className={cn(
                        "flex h-10 w-10 items-center justify-center rounded-md text-xs font-medium transition-all",
                        isSelected
                          ? "bg-primary text-primary-foreground ring-2 ring-primary/50 scale-105"
                          : hasComponents
                            ? "bg-blue-400 dark:bg-blue-500 text-white hover:bg-blue-500 dark:hover:bg-blue-400"
                            : "bg-muted text-muted-foreground hover:bg-muted/80"
                      )}
                    >
                      {String(c + 1).padStart(2, "0")}
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
            </div>
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
                  <h4 className="mb-2 text-sm font-medium">Compartments</h4>
                  {selectedBinComponents.length === 0 ? (
                    <p className="text-sm text-muted-foreground">
                      No components in this bin
                    </p>
                  ) : (
                    <div className="space-y-2">
                      {selectedBinComponents.map((comp) => (
                        <div
                          key={comp.id}
                          className="rounded-lg border bg-background p-3"
                        >
                          <div className="flex items-center justify-between">
                            <p className="text-sm font-medium">{comp.name}</p>
                            <Badge variant="secondary" className="text-xs">
                              {comp.compartment}
                            </Badge>
                          </div>
                          <p className="mt-1 text-sm text-muted-foreground">
                            Qty: {comp.quantity}
                          </p>
                        </div>
                      ))}
                    </div>
                  )}
                </div>

                <div className="space-y-2">
                  <Button variant="outline" className="w-full" size="sm">
                    Add Component
                  </Button>
                  <Button variant="outline" className="w-full" size="sm">
                    Add Compartment
                  </Button>
                </div>
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
