"use client";

import * as React from "react";
import { cn } from "@/lib/utils";

interface ColumnDef {
  key: string;
  label: string;
  defaultWidth: number;
  minWidth?: number;
  className?: string;
  resizable?: boolean;
}

interface ResizableTableProps {
  columns: ColumnDef[];
  children: (columnWidths: Record<string, number>) => React.ReactNode;
  className?: string;
}

function ResizableTable({ columns, children, className }: ResizableTableProps) {
  const [columnWidths, setColumnWidths] = React.useState<Record<string, number>>(() => {
    const widths: Record<string, number> = {};
    for (const col of columns) {
      widths[col.key] = col.defaultWidth;
    }
    return widths;
  });

  const resizing = React.useRef<{
    key: string;
    startX: number;
    startWidth: number;
  } | null>(null);

  const handleMouseDown = React.useCallback(
    (key: string, e: React.MouseEvent) => {
      e.preventDefault();
      resizing.current = {
        key,
        startX: e.clientX,
        startWidth: columnWidths[key],
      };

      const col = columns.find((c) => c.key === key);
      const minWidth = col?.minWidth ?? 50;
      const startX = e.clientX;
      const startWidth = columnWidths[key];

      const handleMouseMove = (moveEvent: MouseEvent) => {
        const delta = moveEvent.clientX - startX;
        const newWidth = Math.max(minWidth, startWidth + delta);
        setColumnWidths((prev) => ({
          ...prev,
          [key]: newWidth,
        }));
      };

      const handleMouseUp = () => {
        resizing.current = null;
        document.removeEventListener("mousemove", handleMouseMove);
        document.removeEventListener("mouseup", handleMouseUp);
        document.body.style.cursor = "";
        document.body.style.userSelect = "";
      };

      document.body.style.cursor = "col-resize";
      document.body.style.userSelect = "none";
      document.addEventListener("mousemove", handleMouseMove);
      document.addEventListener("mouseup", handleMouseUp);
    },
    [columnWidths, columns]
  );

  return (
    <div className="relative w-full overflow-x-auto" data-slot="table-container">
      <table
        data-slot="table"
        className={cn("w-full caption-bottom text-sm", className)}
        style={{ tableLayout: "fixed" }}
      >
        <colgroup>
          {columns.map((col) => (
            <col key={col.key} style={{ width: columnWidths[col.key] }} />
          ))}
        </colgroup>
        <thead data-slot="table-header" className="[&_tr]:border-b">
          <tr
            data-slot="table-row"
            className="hover:bg-muted/50 data-[state=selected]:bg-muted border-b transition-colors"
          >
            {columns.map((col) => (
              <th
                key={col.key}
                data-slot="table-head"
                className={cn(
                  "text-foreground relative h-10 px-2 text-left align-middle font-medium whitespace-nowrap [&:has([role=checkbox])]:pr-0",
                  col.className
                )}
                style={{ width: columnWidths[col.key] }}
              >
                {col.label}
                {col.resizable !== false && (
                  <div
                    className="absolute -right-1.5 top-0 z-10 flex h-full w-3 cursor-col-resize select-none items-center justify-center group"
                    onMouseDown={(e) => handleMouseDown(col.key, e)}
                  >
                    <div className="flex h-5 flex-col items-center justify-center gap-[2px] opacity-40 transition-opacity group-hover:opacity-100">
                      <div className="h-[3px] w-[3px] rounded-full bg-muted-foreground" />
                      <div className="h-[3px] w-[3px] rounded-full bg-muted-foreground" />
                      <div className="h-[3px] w-[3px] rounded-full bg-muted-foreground" />
                    </div>
                  </div>
                )}
              </th>
            ))}
          </tr>
        </thead>
        {children(columnWidths)}
      </table>
    </div>
  );
}

export { ResizableTable };
export type { ColumnDef };
