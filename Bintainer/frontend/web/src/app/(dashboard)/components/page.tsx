"use client";

import { useState, useMemo } from "react";
import { Search, Plus, Pencil, Trash2, ArrowRightLeft, Filter } from "lucide-react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { demoComponents, categories } from "@/lib/demo-data";

export default function ComponentsPage() {
  const [search, setSearch] = useState("");
  const [categoryFilter, setCategoryFilter] = useState<string | null>(null);
  const [lowStockOnly, setLowStockOnly] = useState(false);

  const filtered = useMemo(() => {
    return demoComponents.filter((c) => {
      const matchesSearch =
        !search ||
        c.name.toLowerCase().includes(search.toLowerCase()) ||
        c.category.toLowerCase().includes(search.toLowerCase());
      const matchesCategory = !categoryFilter || c.category === categoryFilter;
      const matchesLowStock = !lowStockOnly || c.quantity <= c.lowStockThreshold;
      return matchesSearch && matchesCategory && matchesLowStock;
    });
  }, [search, categoryFilter, lowStockOnly]);

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold">Components</h1>
          <p className="text-muted-foreground">
            Manage all your components ({demoComponents.length} total)
          </p>
        </div>
        <Button className="gap-2">
          <Plus className="h-4 w-4" />
          Add Component
        </Button>
      </div>

      {/* Search & Filters */}
      <div className="flex flex-wrap items-center gap-3">
        <div className="relative flex-1 min-w-[240px] max-w-md">
          <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
          <Input
            placeholder="Search components..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
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
        <Button
          variant={lowStockOnly ? "default" : "outline"}
          className={lowStockOnly ? "" : "bg-card"}
          onClick={() => setLowStockOnly(!lowStockOnly)}
        >
          Low Stock
        </Button>
      </div>

      {/* Components Table */}
      <div className="rounded-xl border bg-card shadow-sm">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Component</TableHead>
              <TableHead>Category</TableHead>
              <TableHead>Storage Unit</TableHead>
              <TableHead>Bin</TableHead>
              <TableHead>Compartment</TableHead>
              <TableHead className="text-right">Quantity</TableHead>
              <TableHead className="w-[80px]" />
            </TableRow>
          </TableHeader>
          <TableBody>
            {filtered.length === 0 ? (
              <TableRow>
                <TableCell colSpan={7} className="h-24 text-center text-muted-foreground">
                  No components found.
                </TableCell>
              </TableRow>
            ) : (
              filtered.map((comp) => {
                const isLow = comp.quantity <= comp.lowStockThreshold;
                return (
                  <TableRow key={comp.id}>
                    <TableCell className="font-medium">{comp.name}</TableCell>
                    <TableCell>
                      <Badge variant="secondary">{comp.category}</Badge>
                    </TableCell>
                    <TableCell>{comp.storageUnit}</TableCell>
                    <TableCell className="font-mono text-sm">{comp.bin}</TableCell>
                    <TableCell className="font-mono text-sm">{comp.compartment}</TableCell>
                    <TableCell className="text-right">
                      <span className={isLow ? "font-semibold text-destructive" : ""}>
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
                          <DropdownMenuItem>
                            <Pencil className="mr-2 h-3.5 w-3.5" /> Edit
                          </DropdownMenuItem>
                          <DropdownMenuItem>
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
        </Table>
      </div>
    </div>
  );
}
