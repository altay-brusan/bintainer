"use client";

import { useState } from "react";
import type { Bin } from "@/types/api";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { Badge } from "@/components/ui/badge";

interface StorageUnitGridProps {
  columns: number;
  rows: number;
  bins: Bin[];
}

function getBinLabel(column: number, row: number): string {
  const colLetter = String.fromCharCode(65 + column);
  return `${colLetter}${row + 1}`;
}

export function StorageUnitGrid({ columns, rows, bins }: StorageUnitGridProps) {
  const [openBinId, setOpenBinId] = useState<string | null>(null);

  const binMap = new Map<string, Bin>();
  for (const bin of bins) {
    binMap.set(`${bin.column}-${bin.row}`, bin);
  }

  return (
    <div
      className="grid gap-2"
      style={{
        gridTemplateColumns: `repeat(${columns}, minmax(0, 1fr))`,
      }}
    >
      {Array.from({ length: rows }).map((_, row) =>
        Array.from({ length: columns }).map((_, col) => {
          const bin = binMap.get(`${col}-${row}`);
          const label = getBinLabel(col, row);
          const hasCompartments =
            bin && bin.compartments && bin.compartments.length > 0;

          return (
            <Popover
              key={`${col}-${row}`}
              open={openBinId === `${col}-${row}`}
              onOpenChange={(open) =>
                setOpenBinId(open ? `${col}-${row}` : null)
              }
            >
              <PopoverTrigger asChild>
                <button
                  className={`flex flex-col items-center justify-center rounded-lg border-2 p-3 text-sm font-medium transition-colors ${
                    hasCompartments
                      ? "border-blue-300 bg-blue-50 text-blue-700 hover:bg-blue-100 dark:border-blue-700 dark:bg-blue-950 dark:text-blue-300 dark:hover:bg-blue-900"
                      : "border-muted bg-muted/30 text-muted-foreground hover:bg-muted/50"
                  }`}
                >
                  <span className="text-base font-semibold">{label}</span>
                  {hasCompartments && (
                    <span className="text-xs opacity-75">
                      {bin.compartments.length} comp.
                    </span>
                  )}
                </button>
              </PopoverTrigger>
              <PopoverContent className="w-64" align="center">
                <div className="space-y-2">
                  <h4 className="font-semibold">Bin {label}</h4>
                  {bin && bin.compartments && bin.compartments.length > 0 ? (
                    <div className="space-y-1">
                      <p className="text-sm text-muted-foreground">
                        Compartments:
                      </p>
                      <div className="flex flex-wrap gap-1">
                        {bin.compartments.map((comp) => (
                          <Badge key={comp.id} variant="secondary">
                            {comp.label}
                          </Badge>
                        ))}
                      </div>
                    </div>
                  ) : (
                    <p className="text-sm text-muted-foreground">
                      No compartments
                    </p>
                  )}
                </div>
              </PopoverContent>
            </Popover>
          );
        })
      )}
    </div>
  );
}
