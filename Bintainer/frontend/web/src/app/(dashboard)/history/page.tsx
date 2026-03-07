"use client";

import { useState, useMemo } from "react";
import { Search, Filter, Plus, Minus, RefreshCw, ArrowRightLeft } from "lucide-react";
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
import { cn } from "@/lib/utils";
import { demoMovements } from "@/lib/demo-data";

const actionConfig = {
  added: { icon: Plus, label: "Added", className: "bg-green-100 text-green-700 dark:bg-green-900/50 dark:text-green-400" },
  used: { icon: Minus, label: "Used", className: "bg-red-100 text-red-700 dark:bg-red-900/50 dark:text-red-400" },
  restocked: { icon: RefreshCw, label: "Restocked", className: "bg-blue-100 text-blue-700 dark:bg-blue-900/50 dark:text-blue-400" },
  moved: { icon: ArrowRightLeft, label: "Moved", className: "bg-amber-100 text-amber-700 dark:bg-amber-900/50 dark:text-amber-400" },
};

function formatDate(dateStr: string) {
  const date = new Date(dateStr);
  const now = new Date();
  const diffMs = now.getTime() - date.getTime();
  const diffHours = diffMs / (1000 * 60 * 60);

  if (diffHours < 1) return "Just now";
  if (diffHours < 24) return `${Math.floor(diffHours)} hours ago`;
  if (diffHours < 48) return "Yesterday";
  return date.toLocaleDateString("en-US", { month: "short", day: "numeric", year: "numeric" });
}

export default function MovementHistoryPage() {
  const [search, setSearch] = useState("");
  const [actionFilter, setActionFilter] = useState<string | null>(null);

  const filtered = useMemo(() => {
    return demoMovements.filter((m) => {
      const matchesSearch =
        !search || m.component.toLowerCase().includes(search.toLowerCase());
      const matchesAction = !actionFilter || m.action === actionFilter;
      return matchesSearch && matchesAction;
    });
  }, [search, actionFilter]);

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold">Movement History</h1>
        <p className="text-muted-foreground">
          Track all component movements ({demoMovements.length} records)
        </p>
      </div>

      {/* Filters */}
      <div className="flex flex-wrap items-center gap-3">
        <div className="relative flex-1 min-w-[240px] max-w-md">
          <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
          <Input
            placeholder="Search by component..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            className="pl-9 bg-card"
          />
        </div>
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant="outline" className="gap-2 bg-card">
              <Filter className="h-4 w-4" />
              {actionFilter ? actionConfig[actionFilter as keyof typeof actionConfig].label : "All Actions"}
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent>
            <DropdownMenuItem onClick={() => setActionFilter(null)}>
              All Actions
            </DropdownMenuItem>
            {Object.entries(actionConfig).map(([key, config]) => (
              <DropdownMenuItem key={key} onClick={() => setActionFilter(key)}>
                {config.label}
              </DropdownMenuItem>
            ))}
          </DropdownMenuContent>
        </DropdownMenu>
      </div>

      {/* Table */}
      <div className="rounded-xl border bg-card shadow-sm">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Date</TableHead>
              <TableHead>Component</TableHead>
              <TableHead>Action</TableHead>
              <TableHead className="text-right">Quantity</TableHead>
              <TableHead>Location</TableHead>
              <TableHead>User</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {filtered.length === 0 ? (
              <TableRow>
                <TableCell colSpan={6} className="h-24 text-center text-muted-foreground">
                  No movements found.
                </TableCell>
              </TableRow>
            ) : (
              filtered.map((movement) => {
                const config = actionConfig[movement.action];
                return (
                  <TableRow key={movement.id}>
                    <TableCell className="text-muted-foreground">
                      {formatDate(movement.date)}
                    </TableCell>
                    <TableCell className="font-medium">
                      {movement.component}
                    </TableCell>
                    <TableCell>
                      <Badge variant="secondary" className={cn("gap-1", config.className)}>
                        <config.icon className="h-3 w-3" />
                        {config.label}
                      </Badge>
                    </TableCell>
                    <TableCell className="text-right font-mono">
                      <span
                        className={cn(
                          movement.quantity > 0
                            ? "text-green-600 dark:text-green-400"
                            : movement.quantity < 0
                              ? "text-red-600 dark:text-red-400"
                              : "text-muted-foreground"
                        )}
                      >
                        {movement.quantity > 0 ? "+" : ""}
                        {movement.quantity}
                      </span>
                    </TableCell>
                    <TableCell className="font-mono text-sm">
                      {movement.location}
                    </TableCell>
                    <TableCell className="text-muted-foreground">
                      {movement.user}
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
