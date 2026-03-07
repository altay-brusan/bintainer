"use client";

import { useState, useMemo, useEffect, lazy, Suspense } from "react";
import {
  Search,
  SlidersHorizontal,
  MapPin,
  Box,
  Grid3x3,
  Tag,
  Package,
  ChevronDown,
  ChevronUp,
  X,
} from "lucide-react";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { cn } from "@/lib/utils";
import {
  demoComponents,
  demoStorageUnits,
  categories,
  type Component,
} from "@/lib/demo-data";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

const StorageUnit3DViewer = lazy(() =>
  import("@/components/storage-unit-3d-viewer").then((m) => ({
    default: m.StorageUnit3DViewer,
  }))
);

// Group search results by storage unit
interface LocationGroup {
  storageUnitName: string;
  storageUnitId: string;
  rows: number;
  columns: number;
  components: Component[];
}

export default function FindComponentPage() {
  const [search, setSearch] = useState("");
  const [showAdvanced, setShowAdvanced] = useState(false);
  const [categoryFilter, setCategoryFilter] = useState("");
  const [supplierFilter, setSupplierFilter] = useState("");
  const [tagFilter, setTagFilter] = useState("");
  const [packageFilter, setPackageFilter] = useState("");
  const [expandedUnits, setExpandedUnits] = useState<Set<string>>(new Set());
  const [viewModes, setViewModes] = useState<Map<string, "grid" | "3d">>(new Map());

  const allTags = useMemo(() => {
    const set = new Set<string>();
    for (const c of demoComponents) {
      for (const t of c.tags ?? []) set.add(t);
    }
    return Array.from(set).sort();
  }, []);

  const allSuppliers = useMemo(() => {
    const set = new Set<string>();
    for (const c of demoComponents) {
      if (c.supplier) set.add(c.supplier);
    }
    return Array.from(set).sort();
  }, []);

  const allPackages = useMemo(() => {
    const set = new Set<string>();
    for (const c of demoComponents) {
      if (c.package) set.add(c.package);
    }
    return Array.from(set).sort();
  }, []);

  const results = useMemo(() => {
    if (!search.trim() && !categoryFilter && !supplierFilter && !tagFilter && !packageFilter)
      return [];
    return demoComponents.filter((c) => {
      const q = search.toLowerCase();
      const matchesSearch =
        !search.trim() ||
        c.name.toLowerCase().includes(q) ||
        (c.partNumber ?? "").toLowerCase().includes(q) ||
        c.category.toLowerCase().includes(q) ||
        c.bin.toLowerCase().includes(q) ||
        (c.tags ?? []).some((t) => t.toLowerCase().includes(q)) ||
        (c.package ?? "").toLowerCase().includes(q) ||
        (c.supplier ?? "").toLowerCase().includes(q);
      const matchesCategory = !categoryFilter || c.category === categoryFilter;
      const matchesSupplier = !supplierFilter || c.supplier === supplierFilter;
      const matchesTag = !tagFilter || (c.tags ?? []).includes(tagFilter);
      const matchesPackage = !packageFilter || c.package === packageFilter;
      return matchesSearch && matchesCategory && matchesSupplier && matchesTag && matchesPackage;
    });
  }, [search, categoryFilter, supplierFilter, tagFilter, packageFilter]);

  // Group results by storage unit
  const locationGroups = useMemo<LocationGroup[]>(() => {
    const map = new Map<string, Component[]>();
    for (const comp of results) {
      const existing = map.get(comp.storageUnit) ?? [];
      existing.push(comp);
      map.set(comp.storageUnit, existing);
    }
    return Array.from(map.entries()).map(([name, components]) => {
      const su = demoStorageUnits.find((s) => s.name === name);
      return {
        storageUnitName: name,
        storageUnitId: su?.id ?? "",
        rows: su?.rows ?? 1,
        columns: su?.columns ?? 1,
        components,
      };
    });
  }, [results]);

  const hasActiveFilters = categoryFilter || supplierFilter || tagFilter || packageFilter;

  const clearFilters = () => {
    setCategoryFilter("");
    setSupplierFilter("");
    setTagFilter("");
    setPackageFilter("");
  };

  const toggleUnit = (name: string) => {
    setExpandedUnits((prev) => {
      const next = new Set(prev);
      if (next.has(name)) next.delete(name);
      else next.add(name);
      return next;
    });
  };

  const getViewMode = (name: string) => viewModes.get(name) ?? "grid";
  const setViewMode = (name: string, mode: "grid" | "3d") => {
    setViewModes((prev) => new Map(prev).set(name, mode));
  };

  // Auto-expand all groups when results change
  useEffect(() => {
    setExpandedUnits(new Set(locationGroups.map((g) => g.storageUnitName)));
  }, [locationGroups]);

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold">Find Component</h1>
        <p className="text-muted-foreground">
          Search and locate components across all storage units
        </p>
      </div>

      {/* Search Bar */}
      <div className="space-y-3">
        <div className="flex gap-3">
          <div className="relative flex-1">
            <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
            <Input
              placeholder="Search by name, category, tag, package, supplier, bin..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              className="pl-9 bg-card text-base h-12"
            />
          </div>
          <Button
            variant={showAdvanced ? "default" : "outline"}
            className="h-12 gap-2"
            onClick={() => setShowAdvanced(!showAdvanced)}
          >
            <SlidersHorizontal className="h-4 w-4" />
            Filters
            {hasActiveFilters && (
              <Badge variant="secondary" className="ml-1 h-5 w-5 rounded-full p-0 text-[10px] flex items-center justify-center">
                !
              </Badge>
            )}
          </Button>
        </div>

        {/* Advanced Filters */}
        {showAdvanced && (
          <div className="rounded-xl border bg-card p-4 shadow-sm space-y-4">
            <div className="flex items-center justify-between">
              <h3 className="text-sm font-semibold">Advanced Filters</h3>
              {hasActiveFilters && (
                <Button variant="ghost" size="sm" onClick={clearFilters} className="h-7 text-xs gap-1">
                  <X className="h-3 w-3" /> Clear all
                </Button>
              )}
            </div>
            <div className="grid grid-cols-2 gap-3 md:grid-cols-4">
              <div>
                <Label className="text-xs">Category</Label>
                <Select value={categoryFilter || "__all__"} onValueChange={(v) => setCategoryFilter(v === "__all__" ? "" : v)}>
                  <SelectTrigger className="mt-1">
                    <SelectValue placeholder="All categories" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="__all__">All categories</SelectItem>
                    {categories.map((cat) => (
                      <SelectItem key={cat} value={cat}>{cat}</SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              <div>
                <Label className="text-xs">Supplier</Label>
                <Select value={supplierFilter || "__all__"} onValueChange={(v) => setSupplierFilter(v === "__all__" ? "" : v)}>
                  <SelectTrigger className="mt-1">
                    <SelectValue placeholder="All suppliers" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="__all__">All suppliers</SelectItem>
                    {allSuppliers.map((s) => (
                      <SelectItem key={s} value={s}>{s}</SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              <div>
                <Label className="text-xs">Tag</Label>
                <Select value={tagFilter || "__all__"} onValueChange={(v) => setTagFilter(v === "__all__" ? "" : v)}>
                  <SelectTrigger className="mt-1">
                    <SelectValue placeholder="All tags" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="__all__">All tags</SelectItem>
                    {allTags.map((t) => (
                      <SelectItem key={t} value={t}>{t}</SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              <div>
                <Label className="text-xs">Package / Footprint</Label>
                <Select value={packageFilter || "__all__"} onValueChange={(v) => setPackageFilter(v === "__all__" ? "" : v)}>
                  <SelectTrigger className="mt-1">
                    <SelectValue placeholder="All packages" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="__all__">All packages</SelectItem>
                    {allPackages.map((p) => (
                      <SelectItem key={p} value={p}>{p}</SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
            </div>
          </div>
        )}
      </div>

      {/* No results */}
      {(search.trim() || hasActiveFilters) && results.length === 0 && (
        <div className="rounded-xl border border-dashed bg-card p-8 text-center">
          <p className="text-muted-foreground">No components found</p>
        </div>
      )}

      {/* Results summary */}
      {results.length > 0 && (
        <div className="flex items-center gap-3 text-sm text-muted-foreground">
          <span>
            Found <span className="font-semibold text-foreground">{results.length}</span> component{results.length !== 1 ? "s" : ""} in{" "}
            <span className="font-semibold text-foreground">{locationGroups.length}</span> storage unit{locationGroups.length !== 1 ? "s" : ""}
          </span>
        </div>
      )}

      {/* Location Groups */}
      <div className="space-y-6">
        {locationGroups.map((group) => {
          const isExpanded = expandedUnits.has(group.storageUnitName);
          const viewMode = getViewMode(group.storageUnitName);
          const highlightBins = group.components.map((c) => ({
            row: parseInt(c.bin.match(/R(\d+)/)?.[1] ?? "0", 10) - 1,
            col: parseInt(c.bin.match(/C(\d+)/)?.[1] ?? "0", 10) - 1,
          }));
          const highlightSet = new Set(highlightBins.map((b) => `${b.row}-${b.col}`));

          return (
            <div key={group.storageUnitName} className="rounded-xl border bg-card shadow-sm overflow-hidden">
              {/* Group header */}
              <button
                onClick={() => toggleUnit(group.storageUnitName)}
                className="flex w-full items-center justify-between p-4 text-left hover:bg-muted/50 transition-colors"
              >
                <div className="flex items-center gap-3">
                  <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-primary/10">
                    <MapPin className="h-5 w-5 text-primary" />
                  </div>
                  <div>
                    <h3 className="font-semibold">{group.storageUnitName}</h3>
                    <p className="text-sm text-muted-foreground">
                      {group.components.length} component{group.components.length !== 1 ? "s" : ""} found &middot; {group.rows}&times;{group.columns} grid
                    </p>
                  </div>
                </div>
                {isExpanded ? (
                  <ChevronUp className="h-5 w-5 text-muted-foreground" />
                ) : (
                  <ChevronDown className="h-5 w-5 text-muted-foreground" />
                )}
              </button>

              {isExpanded && (
                <div className="border-t px-4 pb-4 space-y-4">
                  {/* Component cards */}
                  <div className="grid gap-3 pt-4 md:grid-cols-2">
                    {group.components.map((comp) => (
                      <div
                        key={comp.id}
                        className="rounded-lg border bg-background p-4 space-y-2"
                      >
                        <div className="flex items-start justify-between">
                          <div>
                            <p className="font-semibold text-[0.95rem]">{comp.name}</p>
                            <p className="text-sm text-muted-foreground">
                              {comp.category}
                              {comp.package && <> &middot; <Package className="inline h-3 w-3 -mt-0.5" /> {comp.package}</>}
                            </p>
                          </div>
                          <Badge variant={comp.quantity <= comp.lowStockThreshold ? "destructive" : "secondary"}>
                            Qty: {comp.quantity}
                          </Badge>
                        </div>

                        {/* Location details */}
                        <div className="flex flex-wrap gap-2 text-sm">
                          <div className="rounded-md bg-primary/10 px-2.5 py-1">
                            <span className="text-muted-foreground">Unit </span>
                            <span className="font-semibold text-primary">{comp.storageUnit}</span>
                          </div>
                          <div className="rounded-md bg-muted px-2.5 py-1">
                            <span className="text-muted-foreground">Bin </span>
                            <span className="font-mono font-semibold">{comp.bin}</span>
                          </div>
                          <div className="rounded-md bg-muted px-2.5 py-1">
                            <span className="text-muted-foreground">Compartment </span>
                            <span className="font-mono font-semibold">{comp.compartment}</span>
                          </div>
                        </div>

                        {/* Tags */}
                        {(comp.tags ?? []).length > 0 && (
                          <div className="flex flex-wrap gap-1">
                            {(comp.tags ?? []).map((tag) => (
                              <span
                                key={tag}
                                className="inline-flex items-center gap-0.5 rounded-full bg-muted px-2 py-0.5 text-xs text-muted-foreground"
                              >
                                <Tag className="h-2.5 w-2.5" />
                                {tag}
                              </span>
                            ))}
                          </div>
                        )}

                        {comp.supplier && (
                          <p className="text-xs text-muted-foreground">
                            Supplier: {comp.supplier}
                          </p>
                        )}
                      </div>
                    ))}
                  </div>

                  {/* View toggle + Grid/3D */}
                  <div className="space-y-3">
                    <div className="flex items-center justify-between">
                      <h4 className="text-sm font-semibold text-muted-foreground">
                        Storage Unit Map
                      </h4>
                      <div className="flex rounded-lg border p-0.5">
                        <Button
                          variant={viewMode === "grid" ? "default" : "ghost"}
                          size="sm"
                          className="gap-1.5 h-7 px-2.5 text-xs"
                          onClick={() => setViewMode(group.storageUnitName, "grid")}
                        >
                          <Grid3x3 className="h-3 w-3" />
                          2D
                        </Button>
                        <Button
                          variant={viewMode === "3d" ? "default" : "ghost"}
                          size="sm"
                          className="gap-1.5 h-7 px-2.5 text-xs"
                          onClick={() => setViewMode(group.storageUnitName, "3d")}
                        >
                          <Box className="h-3 w-3" />
                          3D
                        </Button>
                      </div>
                    </div>

                    {viewMode === "grid" && (
                      <div className="rounded-lg border bg-background p-4">
                        {/* Column headers */}
                        <div className="flex gap-1.5 mb-1.5">
                          <div className="w-8" />
                          {Array.from({ length: group.columns }).map((_, c) => (
                            <div
                              key={c}
                              className="flex h-6 w-8 items-center justify-center text-xs font-medium text-muted-foreground"
                            >
                              {String(c + 1).padStart(2, "0")}
                            </div>
                          ))}
                        </div>

                        {/* Grid */}
                        {Array.from({ length: group.rows }).map((_, r) => (
                          <div key={r} className="flex gap-1.5 mb-1.5">
                            <div className="flex w-8 items-center justify-center text-xs font-medium text-muted-foreground">
                              {String(r + 1).padStart(2, "0")}
                            </div>
                            {Array.from({ length: group.columns }).map((_, c) => {
                              const isHighlighted = highlightSet.has(`${r}-${c}`);
                              return (
                                <div
                                  key={c}
                                  className={cn(
                                    "flex h-8 w-8 items-center justify-center rounded-md text-xs font-medium transition-all",
                                    isHighlighted
                                      ? "bg-yellow-400 text-yellow-900 ring-2 ring-yellow-300 dark:bg-yellow-500 dark:ring-yellow-600 shadow-lg scale-110"
                                      : "bg-muted text-muted-foreground"
                                  )}
                                >
                                  {isHighlighted && <MapPin className="h-3.5 w-3.5" />}
                                </div>
                              );
                            })}
                          </div>
                        ))}

                        <div className="mt-3 flex items-center gap-4 text-xs text-muted-foreground">
                          <div className="flex items-center gap-1.5">
                            <div className="h-3 w-3 rounded-sm bg-yellow-400" /> Found here
                          </div>
                          <div className="flex items-center gap-1.5">
                            <div className="h-3 w-3 rounded-sm bg-muted" /> Other bins
                          </div>
                        </div>
                      </div>
                    )}

                    {viewMode === "3d" && (
                      <Suspense
                        fallback={
                          <div className="flex h-[400px] items-center justify-center rounded-lg border bg-slate-900">
                            <p className="text-sm text-slate-400">Loading 3D viewer...</p>
                          </div>
                        }
                      >
                        <StorageUnit3DViewer
                          occupiedColor="#FACC15"
                          occupiedHoverColor="#FDE047"
                          rows={group.rows}
                          columns={group.columns}
                          bins={Array.from({ length: group.rows }).flatMap((_, r) =>
                            Array.from({ length: group.columns }).map((_, c) => ({
                              id: `${r}-${c}`,
                              row: r,
                              col: c,
                              hasComponents: highlightSet.has(`${r}-${c}`),
                            }))
                          )}
                          selectedBin={null}
                          onBinSelect={() => {}}
                        />
                      </Suspense>
                    )}
                  </div>
                </div>
              )}
            </div>
          );
        })}
      </div>
    </div>
  );
}
