"use client";

import { useState, useMemo } from "react";
import { Search } from "lucide-react";
import { Input } from "@/components/ui/input";
import { cn } from "@/lib/utils";
import { demoComponents, demoStorageUnits } from "@/lib/demo-data";

export default function FindComponentPage() {
  const [search, setSearch] = useState("");

  const results = useMemo(() => {
    if (!search.trim()) return [];
    return demoComponents.filter(
      (c) =>
        c.name.toLowerCase().includes(search.toLowerCase()) ||
        c.category.toLowerCase().includes(search.toLowerCase())
    );
  }, [search]);

  const selectedComponent = results.length > 0 ? results[0] : null;

  const storageUnit = selectedComponent
    ? demoStorageUnits.find((su) => su.name === selectedComponent.storageUnit)
    : null;

  const highlightBin = selectedComponent
    ? {
        row: parseInt(selectedComponent.bin.split("-")[0].replace("R", "")) - 1,
        col: parseInt(selectedComponent.bin.split("-")[1].replace("C", "")) - 1,
      }
    : null;

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold">Find Component</h1>
        <p className="text-muted-foreground">
          Search and locate components in your storage
        </p>
      </div>

      {/* Search */}
      <div className="relative max-w-lg">
        <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
        <Input
          placeholder="Search for a component... (e.g. 10k resistor)"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="pl-9 bg-card text-base h-12"
        />
      </div>

      {search.trim() && results.length === 0 && (
        <div className="rounded-xl border border-dashed bg-card p-8 text-center">
          <p className="text-muted-foreground">No components found for &quot;{search}&quot;</p>
        </div>
      )}

      {/* Results */}
      {results.length > 0 && (
        <div className="grid gap-6 lg:grid-cols-[1fr_1fr]">
          {/* Result List */}
          <div className="space-y-3">
            <h2 className="text-lg font-semibold">
              Results ({results.length})
            </h2>
            <div className="space-y-2">
              {results.map((comp) => (
                <div
                  key={comp.id}
                  className={cn(
                    "rounded-xl border bg-card p-4 transition-colors",
                    comp.id === selectedComponent?.id
                      ? "border-primary ring-1 ring-primary"
                      : "hover:border-primary/50"
                  )}
                >
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="font-semibold">{comp.name}</p>
                      <p className="text-sm text-muted-foreground">
                        {comp.category} &middot; {comp.package}
                      </p>
                    </div>
                    <div className="text-right">
                      <p className="text-sm font-medium">Qty: {comp.quantity}</p>
                    </div>
                  </div>
                  <div className="mt-2 flex gap-4 text-sm text-muted-foreground">
                    <span>Storage: <span className="font-medium text-foreground">{comp.storageUnit}</span></span>
                    <span>Bin: <span className="font-mono font-medium text-foreground">{comp.bin}</span></span>
                    <span>Comp: <span className="font-mono font-medium text-foreground">{comp.compartment}</span></span>
                  </div>
                </div>
              ))}
            </div>
          </div>

          {/* Grid Visualization */}
          {storageUnit && highlightBin && (
            <div className="space-y-3">
              <h2 className="text-lg font-semibold">
                {storageUnit.name} - Grid View
              </h2>
              <div className="rounded-xl border bg-card p-5 shadow-sm">
                <div className="mb-4 flex gap-4 text-sm text-muted-foreground">
                  <span>Row: <span className="font-semibold text-foreground">{String(highlightBin.row + 1).padStart(2, "0")}</span></span>
                  <span>Column: <span className="font-semibold text-foreground">{String(highlightBin.col + 1).padStart(2, "0")}</span></span>
                  <span>Compartment: <span className="font-semibold text-foreground">{selectedComponent?.compartment}</span></span>
                </div>

                {/* Column headers */}
                <div className="flex gap-1.5 mb-1.5">
                  <div className="w-8" />
                  {Array.from({ length: storageUnit.columns }).map((_, c) => (
                    <div
                      key={c}
                      className="flex h-6 w-8 items-center justify-center text-xs font-medium text-muted-foreground"
                    >
                      {String(c + 1).padStart(2, "0")}
                    </div>
                  ))}
                </div>

                {/* Grid */}
                {Array.from({ length: storageUnit.rows }).map((_, r) => (
                  <div key={r} className="flex gap-1.5 mb-1.5">
                    <div className="flex w-8 items-center justify-center text-xs font-medium text-muted-foreground">
                      {String(r + 1).padStart(2, "0")}
                    </div>
                    {Array.from({ length: storageUnit.columns }).map((_, c) => {
                      const isHighlighted =
                        r === highlightBin.row && c === highlightBin.col;
                      return (
                        <div
                          key={c}
                          className={cn(
                            "flex h-8 w-8 items-center justify-center rounded-md text-xs font-medium transition-colors",
                            isHighlighted
                              ? "bg-green-500 text-white ring-2 ring-green-300 dark:ring-green-700 shadow-lg scale-110"
                              : "bg-muted text-muted-foreground"
                          )}
                        >
                          {isHighlighted ? "!" : ""}
                        </div>
                      );
                    })}
                  </div>
                ))}

                <div className="mt-4 flex items-center gap-4 text-xs text-muted-foreground">
                  <div className="flex items-center gap-1.5">
                    <div className="h-3 w-3 rounded-sm bg-green-500" />
                    Found
                  </div>
                  <div className="flex items-center gap-1.5">
                    <div className="h-3 w-3 rounded-sm bg-muted" />
                    Other bins
                  </div>
                </div>
              </div>
            </div>
          )}
        </div>
      )}
    </div>
  );
}
