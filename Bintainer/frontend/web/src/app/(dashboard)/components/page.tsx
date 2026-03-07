"use client";

import { useState, useMemo } from "react";
import { Search, Plus, Pencil, Trash2, ArrowRightLeft, Filter, Tag, X, ChevronLeft, ChevronRight, FileSpreadsheet, Globe, ChevronDown } from "lucide-react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { TableBody, TableCell, TableRow } from "@/components/ui/table";
import { ResizableTable, type ColumnDef } from "@/components/ui/resizable-table";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { demoComponents, categories, type Component } from "@/lib/demo-data";
import { useCurrency } from "@/lib/currency";
import { AddComponentDialog } from "@/components/add-component-dialog";
import { EditComponentDialog } from "@/components/edit-component-dialog";
import { MoveComponentDialog } from "@/components/move-component-dialog";
import { ImportBomDialog } from "@/components/import-bom-dialog";
import { ImportSupplierDialog } from "@/components/import-supplier-dialog";

const tagColors = [
  "bg-blue-100 text-blue-700 dark:bg-blue-900/40 dark:text-blue-300",
  "bg-emerald-100 text-emerald-700 dark:bg-emerald-900/40 dark:text-emerald-300",
  "bg-amber-100 text-amber-700 dark:bg-amber-900/40 dark:text-amber-300",
  "bg-purple-100 text-purple-700 dark:bg-purple-900/40 dark:text-purple-300",
  "bg-rose-100 text-rose-700 dark:bg-rose-900/40 dark:text-rose-300",
  "bg-cyan-100 text-cyan-700 dark:bg-cyan-900/40 dark:text-cyan-300",
  "bg-orange-100 text-orange-700 dark:bg-orange-900/40 dark:text-orange-300",
  "bg-pink-100 text-pink-700 dark:bg-pink-900/40 dark:text-pink-300",
  "bg-indigo-100 text-indigo-700 dark:bg-indigo-900/40 dark:text-indigo-300",
  "bg-teal-100 text-teal-700 dark:bg-teal-900/40 dark:text-teal-300",
];

const tagColorCache = new Map<string, string>();
let nextColorIndex = 0;

function getTagColor(tag: string): string {
  const cached = tagColorCache.get(tag);
  if (cached) return cached;
  const color = tagColors[nextColorIndex % tagColors.length];
  nextColorIndex++;
  tagColorCache.set(tag, color);
  return color;
}

const columns: ColumnDef[] = [
  { key: "component", label: "Component", defaultWidth: 160, minWidth: 120 },
  { key: "partNumber", label: "Part Number", defaultWidth: 160, minWidth: 100 },
  { key: "unitPrice", label: "Unit Price", defaultWidth: 90, minWidth: 70 },
  { key: "category", label: "Category", defaultWidth: 130, minWidth: 100 },
  { key: "tags", label: "Tags", defaultWidth: 160, minWidth: 100 },
  { key: "storageUnit", label: "Storage Unit", defaultWidth: 130, minWidth: 100 },
  { key: "bin", label: "Bin", defaultWidth: 100, minWidth: 70 },
  { key: "compartment", label: "Compartment", defaultWidth: 110, minWidth: 70 },
  { key: "quantity", label: "Quantity", defaultWidth: 100, minWidth: 80 },
  { key: "actions", label: "", defaultWidth: 60, minWidth: 50, resizable: false },
];

export default function ComponentsPage() {
  const [search, setSearch] = useState("");
  const [categoryFilter, setCategoryFilter] = useState<string | null>(null);
  const [tagFilter, setTagFilter] = useState<string | null>(null);
  const [lowStockOnly, setLowStockOnly] = useState(false);
  const [editComponent, setEditComponent] = useState<Component | null>(null);
  const [moveComponent, setMoveComponent] = useState<Component | null>(null);
  const [importBomOpen, setImportBomOpen] = useState(false);
  const [importSupplierOpen, setImportSupplierOpen] = useState(false);
  const { format: formatPrice } = useCurrency();
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);

  const allTags = useMemo(() => {
    const set = new Set<string>();
    for (const c of demoComponents) {
      for (const t of c.tags ?? []) set.add(t);
    }
    return Array.from(set).sort();
  }, []);

  const filtered = useMemo(() => {
    return demoComponents.filter((c) => {
      const q = search.toLowerCase();
      const matchesSearch =
        !search ||
        c.name.toLowerCase().includes(q) ||
        (c.partNumber ?? "").toLowerCase().includes(q) ||
        c.category.toLowerCase().includes(q) ||
        (c.tags ?? []).some((t) => t.toLowerCase().includes(q));
      const matchesCategory = !categoryFilter || c.category === categoryFilter;
      const matchesTag = !tagFilter || (c.tags ?? []).includes(tagFilter);
      const matchesLowStock = !lowStockOnly || c.quantity <= c.lowStockThreshold;
      return matchesSearch && matchesCategory && matchesTag && matchesLowStock;
    });
  }, [search, categoryFilter, tagFilter, lowStockOnly]);

  // Reset to page 1 when filters change
  const totalPages = Math.max(1, Math.ceil(filtered.length / pageSize));
  const currentPage = Math.min(page, totalPages);
  const paginatedData = useMemo(() => {
    const start = (currentPage - 1) * pageSize;
    return filtered.slice(start, start + pageSize);
  }, [filtered, currentPage, pageSize]);

  // Reset page when filters change
  const resetPage = () => setPage(1);

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold">Components</h1>
          <p className="text-muted-foreground">
            Manage all your components ({demoComponents.length} total)
          </p>
        </div>
        <div className="flex items-center gap-2">
          <AddComponentDialog />
          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button variant="outline" className="gap-2">
                Import
                <ChevronDown className="h-4 w-4" />
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end">
              <DropdownMenuItem onClick={() => setImportBomOpen(true)}>
                <FileSpreadsheet className="mr-2 h-4 w-4" />
                Import from Excel / BOM
              </DropdownMenuItem>
              <DropdownMenuItem onClick={() => setImportSupplierOpen(true)}>
                <Globe className="mr-2 h-4 w-4" />
                Import from Supplier
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      </div>

      {/* Search & Filters */}
      <div className="flex flex-wrap items-center gap-3">
        <div className="relative flex-1 min-w-[240px] max-w-md">
          <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
          <Input
            placeholder="Search components..."
            value={search}
            onChange={(e) => { setSearch(e.target.value); resetPage(); }}
            className="pl-9 bg-card"
          />
        </div>
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant="outline" className="gap-2 bg-card">
              <Filter className="h-4 w-4" />
              {categoryFilter ?? "All Categories"}
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent>
            <DropdownMenuItem onClick={() => setCategoryFilter(null)}>
              All Categories
            </DropdownMenuItem>
            {categories.map((cat) => (
              <DropdownMenuItem key={cat} onClick={() => setCategoryFilter(cat)}>
                {cat}
              </DropdownMenuItem>
            ))}
          </DropdownMenuContent>
        </DropdownMenu>
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant={tagFilter ? "default" : "outline"} className={tagFilter ? "" : "gap-2 bg-card"}>
              <Tag className="h-4 w-4" />
              {tagFilter ?? "All Tags"}
              {tagFilter && (
                <X
                  className="ml-1 h-3.5 w-3.5"
                  onClick={(e) => { e.stopPropagation(); setTagFilter(null); }}
                />
              )}
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent>
            <DropdownMenuItem onClick={() => setTagFilter(null)}>
              All Tags
            </DropdownMenuItem>
            {allTags.map((tag) => (
              <DropdownMenuItem key={tag} onClick={() => setTagFilter(tag)}>
                {tag}
              </DropdownMenuItem>
            ))}
          </DropdownMenuContent>
        </DropdownMenu>
        <Button
          variant={lowStockOnly ? "default" : "outline"}
          className={lowStockOnly ? "" : "bg-card"}
          onClick={() => setLowStockOnly(!lowStockOnly)}
        >
          Low Stock
        </Button>
      </div>

      {/* Components Table */}
      <div className="rounded-xl border bg-card shadow-sm text-sm">
        <ResizableTable columns={columns}>
          {() => (
            <TableBody>
              {paginatedData.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={10} className="h-24 text-center text-muted-foreground">
                    No components found.
                  </TableCell>
                </TableRow>
              ) : (
                paginatedData.map((comp) => {
                  const isLow = comp.quantity <= comp.lowStockThreshold;
                  return (
                    <TableRow key={comp.id}>
                      <TableCell className="font-medium truncate text-[0.9rem]">{comp.name}</TableCell>
                      <TableCell className="font-mono truncate text-[0.85rem] text-muted-foreground">{comp.partNumber ?? "—"}</TableCell>
                      <TableCell className="font-mono text-[0.85rem]">{comp.unitPrice != null ? formatPrice(comp.unitPrice) : "—"}</TableCell>
                      <TableCell className="truncate">
                        <Badge variant="secondary">{comp.category}</Badge>
                      </TableCell>
                      <TableCell>
                        <div className="flex flex-wrap gap-1">
                          {(comp.tags ?? []).map((tag) => (
                            <button
                              key={tag}
                              onClick={() => setTagFilter(tag)}
                              className={`inline-flex items-center gap-1 rounded-full px-2 py-0.5 text-xs font-medium cursor-pointer transition-opacity hover:opacity-80 ${getTagColor(tag)}`}
                            >
                              <Tag className="h-3 w-3" />
                              {tag}
                            </button>
                          ))}
                        </div>
                      </TableCell>
                      <TableCell className="truncate text-[0.9rem]">{comp.storageUnit}</TableCell>
                      <TableCell className="font-mono text-[0.9rem]">{comp.bin}</TableCell>
                      <TableCell className="font-mono text-[0.9rem]">{comp.compartment}</TableCell>
                      <TableCell>
                        <span className={`text-[0.9rem] ${isLow ? "font-semibold text-destructive" : ""}`}>
                          {comp.quantity}
                        </span>
                        {isLow && (
                          <Badge variant="destructive" className="ml-2 text-xs">
                            Low
                          </Badge>
                        )}
                      </TableCell>
                      <TableCell>
                        <DropdownMenu>
                          <DropdownMenuTrigger asChild>
                            <Button variant="ghost" size="icon" className="h-8 w-8">
                              <Pencil className="h-3.5 w-3.5" />
                            </Button>
                          </DropdownMenuTrigger>
                          <DropdownMenuContent align="end">
                            <DropdownMenuItem onClick={() => setEditComponent(comp)}>
                              <Pencil className="mr-2 h-3.5 w-3.5" /> Edit
                            </DropdownMenuItem>
                            <DropdownMenuItem onClick={() => setMoveComponent(comp)}>
                              <ArrowRightLeft className="mr-2 h-3.5 w-3.5" /> Move
                            </DropdownMenuItem>
                            <DropdownMenuItem className="text-destructive">
                              <Trash2 className="mr-2 h-3.5 w-3.5" /> Delete
                            </DropdownMenuItem>
                          </DropdownMenuContent>
                        </DropdownMenu>
                      </TableCell>
                    </TableRow>
                  );
                })
              )}
            </TableBody>
          )}
        </ResizableTable>
      </div>

      {/* Pagination */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-2 text-sm text-muted-foreground">
          <span>
            Showing {filtered.length === 0 ? 0 : (currentPage - 1) * pageSize + 1}–{Math.min(currentPage * pageSize, filtered.length)} of {filtered.length}
          </span>
          <select
            value={pageSize}
            onChange={(e) => { setPageSize(Number(e.target.value)); resetPage(); }}
            className="rounded-md border bg-card px-2 py-1 text-sm"
          >
            <option value={10}>10 / page</option>
            <option value={20}>20 / page</option>
            <option value={50}>50 / page</option>
            <option value={100}>100 / page</option>
          </select>
        </div>
        <div className="flex items-center gap-1">
          <Button
            variant="outline"
            size="icon"
            className="h-8 w-8"
            disabled={currentPage <= 1}
            onClick={() => setPage(currentPage - 1)}
          >
            <ChevronLeft className="h-4 w-4" />
          </Button>
          {Array.from({ length: totalPages }, (_, i) => i + 1)
            .filter((p) => p === 1 || p === totalPages || Math.abs(p - currentPage) <= 1)
            .map((p, idx, arr) => (
              <span key={p} className="flex items-center">
                {idx > 0 && arr[idx - 1] !== p - 1 && (
                  <span className="px-1 text-muted-foreground">...</span>
                )}
                <Button
                  variant={p === currentPage ? "default" : "outline"}
                  size="icon"
                  className="h-8 w-8"
                  onClick={() => setPage(p)}
                >
                  {p}
                </Button>
              </span>
            ))}
          <Button
            variant="outline"
            size="icon"
            className="h-8 w-8"
            disabled={currentPage >= totalPages}
            onClick={() => setPage(currentPage + 1)}
          >
            <ChevronRight className="h-4 w-4" />
          </Button>
        </div>
      </div>

      <EditComponentDialog
        component={editComponent}
        open={!!editComponent}
        onOpenChange={(open) => { if (!open) setEditComponent(null); }}
      />
      <MoveComponentDialog
        component={moveComponent}
        open={!!moveComponent}
        onOpenChange={(open) => { if (!open) setMoveComponent(null); }}
      />
      <ImportBomDialog
        open={importBomOpen}
        onOpenChange={setImportBomOpen}
      />
      <ImportSupplierDialog
        open={importSupplierOpen}
        onOpenChange={setImportSupplierOpen}
      />
    </div>
  );
}
