"use client";

import { useState, useMemo } from "react";
import {
  Search,
  SlidersHorizontal,
  Tag,
  Package,
  X,
  ChevronDown,
  ChevronUp,
  Loader2,
} from "lucide-react";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { useSearchComponents, useComponent } from "@/hooks/use-components";
import { useCategories } from "@/hooks/use-categories";
import { useTags } from "@/hooks/use-tags";
import { useFootprints } from "@/hooks/use-footprints";
import { ComponentLocations } from "@/components/component-locations";
import type { SearchComponentItemResponse, CategoryResponse } from "@/types/api";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

function flattenCategories(cats: CategoryResponse[]): CategoryResponse[] {
  return cats.flatMap((c) => [c, ...flattenCategories(c.children)]);
}

function ExpandedLocations({ componentId }: { componentId: string }) {
  const { data: component, isLoading } = useComponent(componentId);

  if (isLoading) {
    return (
      <div className="flex items-center gap-2 p-3 text-sm text-muted-foreground">
        <Loader2 className="h-4 w-4 animate-spin" />
        Loading locations...
      </div>
    );
  }

  if (!component) return null;

  return (
    <ComponentLocations
      componentId={component.id}
      partNumber={component.partNumber}
      locations={component.locations}
    />
  );
}

export default function FindComponentPage() {
  const [search, setSearch] = useState("");
  const [showAdvanced, setShowAdvanced] = useState(false);
  const [categoryFilter, setCategoryFilter] = useState("");
  const [tagFilter, setTagFilter] = useState("");
  const [footprintFilter, setFootprintFilter] = useState("");
  const [expandedId, setExpandedId] = useState<string | null>(null);

  const { data: categoriesData } = useCategories();
  const { data: tagsData } = useTags();
  const { data: footprintsData } = useFootprints();

  const allCategories = useMemo(
    () => flattenCategories(categoriesData ?? []),
    [categoriesData]
  );
  const allTags = tagsData ?? [];
  const allFootprints = footprintsData ?? [];

  const hasActiveFilters = categoryFilter || tagFilter || footprintFilter;
  const hasSearchOrFilters = search.trim() || hasActiveFilters;

  const { data: searchData, isLoading } = useSearchComponents({
    q: search || undefined,
    categoryId: categoryFilter || undefined,
    tag: tagFilter || undefined,
    footprintId: footprintFilter || undefined,
    page: 1,
    pageSize: 50,
  });

  const results = searchData?.items ?? [];

  const clearFilters = () => {
    setCategoryFilter("");
    setTagFilter("");
    setFootprintFilter("");
  };

  const toggleExpand = (id: string) => {
    setExpandedId(expandedId === id ? null : id);
  };

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
              placeholder="Search by part number, description, manufacturer..."
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
            <div className="grid grid-cols-2 gap-3 md:grid-cols-3">
              <div>
                <Label className="text-xs">Category</Label>
                <Select value={categoryFilter || "__all__"} onValueChange={(v) => setCategoryFilter(v === "__all__" ? "" : v)}>
                  <SelectTrigger className="mt-1">
                    <SelectValue placeholder="All categories" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="__all__">All categories</SelectItem>
                    {allCategories.map((cat) => (
                      <SelectItem key={cat.id} value={cat.id}>{cat.name}</SelectItem>
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
                <Select value={footprintFilter || "__all__"} onValueChange={(v) => setFootprintFilter(v === "__all__" ? "" : v)}>
                  <SelectTrigger className="mt-1">
                    <SelectValue placeholder="All footprints" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="__all__">All footprints</SelectItem>
                    {allFootprints.map((f) => (
                      <SelectItem key={f.id} value={f.id}>{f.name}</SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
            </div>
          </div>
        )}
      </div>

      {/* Loading */}
      {isLoading && hasSearchOrFilters && (
        <div className="rounded-xl border border-dashed bg-card p-8 text-center">
          <p className="text-muted-foreground">Searching...</p>
        </div>
      )}

      {/* No results */}
      {!isLoading && hasSearchOrFilters && results.length === 0 && (
        <div className="rounded-xl border border-dashed bg-card p-8 text-center">
          <p className="text-muted-foreground">No components found</p>
        </div>
      )}

      {/* Results summary */}
      {results.length > 0 && (
        <div className="flex items-center gap-3 text-sm text-muted-foreground">
          <span>
            Found <span className="font-semibold text-foreground">{searchData?.totalCount ?? results.length}</span> component{(searchData?.totalCount ?? results.length) !== 1 ? "s" : ""}
          </span>
        </div>
      )}

      {/* Results as flat card list */}
      {results.length > 0 && (
        <div className="space-y-3">
          <div className="grid gap-3 md:grid-cols-2 lg:grid-cols-3">
            {results.map((comp) => {
              const compTags = comp.tags ? comp.tags.split(",").map((t) => t.trim()).filter(Boolean) : [];
              const isExpanded = expandedId === comp.id;
              return (
                <div
                  key={comp.id}
                  className={`rounded-lg border bg-card shadow-sm overflow-hidden ${
                    isExpanded ? "md:col-span-2 lg:col-span-3" : ""
                  }`}
                >
                  <div className="p-4 space-y-2">
                    <div className="flex items-start justify-between">
                      <div>
                        <p className="font-semibold text-[0.95rem]">{comp.partNumber}</p>
                        <p className="text-sm text-muted-foreground line-clamp-2">
                          {comp.description || "No description"}
                        </p>
                      </div>
                      <Badge variant="secondary">
                        Qty: {comp.totalQuantity}
                      </Badge>
                    </div>

                    {/* Details */}
                    <div className="flex flex-wrap gap-2 text-sm">
                      {comp.categoryName && (
                        <div className="rounded-md bg-primary/10 px-2.5 py-1">
                          <span className="font-medium text-primary">{comp.categoryName}</span>
                        </div>
                      )}
                      {comp.footprintName && (
                        <div className="rounded-md bg-muted px-2.5 py-1">
                          <Package className="inline h-3 w-3 -mt-0.5 mr-1" />
                          <span className="font-mono">{comp.footprintName}</span>
                        </div>
                      )}
                    </div>

                    {/* Tags */}
                    {compTags.length > 0 && (
                      <div className="flex flex-wrap gap-1">
                        {compTags.map((tag) => (
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

                    {comp.manufacturer && (
                      <p className="text-xs text-muted-foreground">
                        Manufacturer: {comp.manufacturer}
                      </p>
                    )}

                    {comp.unitPrice != null && (
                      <p className="text-xs text-muted-foreground">
                        Unit price: ${comp.unitPrice.toFixed(2)}
                      </p>
                    )}

                    {/* Expand button */}
                    <Button
                      variant="ghost"
                      size="sm"
                      className="w-full h-7 text-xs gap-1"
                      onClick={() => toggleExpand(comp.id)}
                    >
                      {isExpanded ? (
                        <>
                          <ChevronUp className="h-3 w-3" />
                          Hide Locations
                        </>
                      ) : (
                        <>
                          <ChevronDown className="h-3 w-3" />
                          Show Locations & Take Out
                        </>
                      )}
                    </Button>
                  </div>

                  {/* Expanded locations panel — full width */}
                  {isExpanded && (
                    <div className="border-t bg-muted/20 p-4">
                      <ExpandedLocations componentId={comp.id} />
                    </div>
                  )}
                </div>
              );
            })}
          </div>
        </div>
      )}
    </div>
  );
}
