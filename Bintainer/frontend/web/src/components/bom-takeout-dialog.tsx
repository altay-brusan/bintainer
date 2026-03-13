"use client";

import { useRef, useState, useCallback } from "react";
import {
  FileSpreadsheet,
  Upload,
  X,
  Loader2,
  CheckCircle2,
  AlertCircle,
  XCircle,
  ChevronRight,
  ChevronLeft,
} from "lucide-react";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { Badge } from "@/components/ui/badge";
import { ScrollArea } from "@/components/ui/scroll-area";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { toast } from "sonner";
import { parseBomFile, type BomLine } from "@/lib/bom-parser";
import api from "@/lib/api";
import type {
  SearchComponentsPagedResponse,
  ComponentResponse,
  ComponentLocationResponse,
} from "@/types/api";
import { useAdjustQuantity } from "@/hooks/use-component-actions";
import { useQueryClient } from "@tanstack/react-query";

interface BomTakeoutDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

type Step = "upload" | "preview" | "confirm" | "results";

interface MatchedBomLine {
  bomLine: BomLine;
  componentId?: string;
  partNumber: string;
  required: number;
  available: number;
  locations: ComponentLocationResponse[];
  status: "ok" | "partial" | "missing";
}

interface Allocation {
  componentId: string;
  partNumber: string;
  compartmentId: string;
  locationLabel: string;
  quantity: number;
}

function allocateFromLocations(
  locations: ComponentLocationResponse[],
  required: number
): { allocations: { compartmentId: string; label: string; qty: number }[]; fulfilled: number } {
  // Greedy: take from compartments with most stock first
  const sorted = [...locations].sort((a, b) => b.quantity - a.quantity);
  const allocations: { compartmentId: string; label: string; qty: number }[] = [];
  let remaining = required;

  for (const loc of sorted) {
    if (remaining <= 0) break;
    const take = Math.min(remaining, loc.quantity);
    if (take > 0) {
      allocations.push({
        compartmentId: loc.compartmentId,
        label: `${loc.storageUnitName} > ${loc.label}`,
        qty: take,
      });
      remaining -= take;
    }
  }

  return { allocations, fulfilled: required - remaining };
}

export function BomTakeoutDialog({ open, onOpenChange }: BomTakeoutDialogProps) {
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [step, setStep] = useState<Step>("upload");
  const [file, setFile] = useState<File | null>(null);
  const [parsing, setParsing] = useState(false);
  const [matchedLines, setMatchedLines] = useState<MatchedBomLine[]>([]);
  const [allocations, setAllocations] = useState<Allocation[]>([]);
  const [progress, setProgress] = useState(0);
  const [results, setResults] = useState<{
    success: number;
    failed: number;
    errors: string[];
  }>({ success: 0, failed: 0, errors: [] });

  const adjustQuantity = useAdjustQuantity();
  const queryClient = useQueryClient();

  const reset = () => {
    setStep("upload");
    setFile(null);
    setParsing(false);
    setMatchedLines([]);
    setAllocations([]);
    setProgress(0);
    setResults({ success: 0, failed: 0, errors: [] });
    if (fileInputRef.current) fileInputRef.current.value = "";
  };

  const handleFileSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
    const f = e.target.files?.[0];
    if (f) setFile(f);
  };

  const handleParse = useCallback(async () => {
    if (!file) return;
    setParsing(true);

    try {
      const bomLines = await parseBomFile(file);
      if (bomLines.length === 0) {
        toast.error("No valid lines found in BOM file");
        setParsing(false);
        return;
      }

      const matched: MatchedBomLine[] = [];

      for (const bomLine of bomLines) {
        try {
          // Search for component by part number
          const { data: searchResult } =
            await api.get<SearchComponentsPagedResponse>(
              "/api/components/search",
              { params: { q: bomLine.partNumber, page: 1, pageSize: 5 } }
            );

          const exactMatch = searchResult.items.find(
            (item) =>
              item.partNumber.toLowerCase() ===
              bomLine.partNumber.toLowerCase()
          );

          if (exactMatch) {
            // Fetch full component to get locations
            const { data: full } = await api.get<ComponentResponse>(
              `/api/components/${exactMatch.id}`
            );
            const totalAvailable = full.locations.reduce(
              (sum, l) => sum + l.quantity,
              0
            );

            matched.push({
              bomLine,
              componentId: full.id,
              partNumber: full.partNumber,
              required: bomLine.quantity,
              available: totalAvailable,
              locations: full.locations,
              status:
                totalAvailable >= bomLine.quantity
                  ? "ok"
                  : totalAvailable > 0
                    ? "partial"
                    : "missing",
            });
          } else {
            matched.push({
              bomLine,
              partNumber: bomLine.partNumber,
              required: bomLine.quantity,
              available: 0,
              locations: [],
              status: "missing",
            });
          }
        } catch {
          matched.push({
            bomLine,
            partNumber: bomLine.partNumber,
            required: bomLine.quantity,
            available: 0,
            locations: [],
            status: "missing",
          });
        }
      }

      setMatchedLines(matched);
      setStep("preview");
    } catch (err) {
      toast.error(
        err instanceof Error ? err.message : "Failed to parse BOM file"
      );
    } finally {
      setParsing(false);
    }
  }, [file]);

  const handlePrepareAllocations = () => {
    const allocs: Allocation[] = [];

    for (const line of matchedLines) {
      if (!line.componentId || line.locations.length === 0) continue;
      const { allocations: lineAllocs } = allocateFromLocations(
        line.locations,
        line.required
      );
      for (const a of lineAllocs) {
        allocs.push({
          componentId: line.componentId,
          partNumber: line.partNumber,
          compartmentId: a.compartmentId,
          locationLabel: a.label,
          quantity: a.qty,
        });
      }
    }

    setAllocations(allocs);
    setStep("confirm");
  };

  const handleExecute = async () => {
    setStep("results");
    setProgress(0);
    let success = 0;
    let failed = 0;
    const errors: string[] = [];

    for (let i = 0; i < allocations.length; i++) {
      const alloc = allocations[i];
      try {
        await new Promise<void>((resolve, reject) => {
          adjustQuantity.mutate(
            {
              id: alloc.componentId,
              compartmentId: alloc.compartmentId,
              action: "Used",
              quantity: alloc.quantity,
              notes: `BOM takeout: ${file?.name}`,
            },
            {
              onSuccess: () => {
                success++;
                resolve();
              },
              onError: (err) => {
                failed++;
                errors.push(
                  `${alloc.partNumber} (${alloc.locationLabel}): ${err instanceof Error ? err.message : "Failed"}`
                );
                resolve(); // continue despite error
              },
            }
          );
        });
      } catch {
        failed++;
        errors.push(`${alloc.partNumber}: unexpected error`);
      }
      setProgress(Math.round(((i + 1) / allocations.length) * 100));
    }

    setResults({ success, failed, errors });
    // Invalidate all relevant queries
    queryClient.invalidateQueries({ queryKey: ["components"] });
    queryClient.invalidateQueries({ queryKey: ["component"] });
    queryClient.invalidateQueries({ queryKey: ["storage-unit"] });

    if (failed === 0) {
      toast.success(`BOM takeout complete: ${success} operations succeeded`);
    } else {
      toast.warning(
        `BOM takeout: ${success} succeeded, ${failed} failed`
      );
    }
  };

  const okCount = matchedLines.filter((l) => l.status === "ok").length;
  const partialCount = matchedLines.filter((l) => l.status === "partial").length;
  const missingCount = matchedLines.filter((l) => l.status === "missing").length;

  return (
    <Dialog
      open={open}
      onOpenChange={(v) => {
        onOpenChange(v);
        if (!v) reset();
      }}
    >
      <DialogContent className="sm:max-w-2xl">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <FileSpreadsheet className="h-5 w-5" />
            Take Out by BOM
          </DialogTitle>
        </DialogHeader>

        {/* Step indicators */}
        <div className="flex items-center gap-2 text-xs text-muted-foreground">
          {(["upload", "preview", "confirm", "results"] as Step[]).map(
            (s, i) => (
              <span key={s} className="flex items-center gap-1">
                {i > 0 && <ChevronRight className="h-3 w-3" />}
                <span
                  className={
                    step === s ? "font-semibold text-foreground" : ""
                  }
                >
                  {i + 1}. {s.charAt(0).toUpperCase() + s.slice(1)}
                </span>
              </span>
            )
          )}
        </div>

        {/* Step 1: Upload */}
        {step === "upload" && (
          <div className="space-y-4">
            <div className="space-y-2">
              <Label>Upload BOM File</Label>
              <div
                className="flex items-center justify-center rounded-lg border-2 border-dashed bg-muted/30 p-8 transition-colors hover:border-primary/50 cursor-pointer"
                onClick={() => fileInputRef.current?.click()}
              >
                {file ? (
                  <div className="flex items-center gap-3">
                    <FileSpreadsheet className="h-8 w-8 text-primary" />
                    <div>
                      <p className="font-medium">{file.name}</p>
                      <p className="text-xs text-muted-foreground">
                        {(file.size / 1024).toFixed(1)} KB
                      </p>
                    </div>
                    <Button
                      variant="ghost"
                      size="icon"
                      className="h-7 w-7"
                      onClick={(e) => {
                        e.stopPropagation();
                        setFile(null);
                      }}
                    >
                      <X className="h-4 w-4" />
                    </Button>
                  </div>
                ) : (
                  <div className="flex flex-col items-center gap-2 text-muted-foreground">
                    <Upload className="h-8 w-8" />
                    <p className="text-sm">
                      Click to upload CSV, XLSX, or XLS file
                    </p>
                    <p className="text-xs">
                      Columns: Part Number + Quantity (auto-detected)
                    </p>
                  </div>
                )}
              </div>
              <input
                ref={fileInputRef}
                type="file"
                accept=".csv,.xlsx,.xls"
                className="hidden"
                onChange={handleFileSelect}
              />
            </div>
            <div className="flex justify-end gap-3">
              <Button
                variant="outline"
                onClick={() => onOpenChange(false)}
              >
                Cancel
              </Button>
              <Button
                onClick={handleParse}
                disabled={!file || parsing}
              >
                {parsing && (
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                )}
                Parse & Match
              </Button>
            </div>
          </div>
        )}

        {/* Step 2: Preview */}
        {step === "preview" && (
          <div className="space-y-4">
            <div className="flex gap-2">
              <Badge
                variant="secondary"
                className="gap-1"
              >
                <CheckCircle2 className="h-3 w-3 text-emerald-500" />
                {okCount} ready
              </Badge>
              <Badge variant="secondary" className="gap-1">
                <AlertCircle className="h-3 w-3 text-amber-500" />
                {partialCount} partial
              </Badge>
              <Badge variant="secondary" className="gap-1">
                <XCircle className="h-3 w-3 text-red-500" />
                {missingCount} missing
              </Badge>
            </div>

            <ScrollArea className="h-[300px]">
              <div className="space-y-1">
                {matchedLines.map((line, i) => (
                  <div
                    key={i}
                    className="flex items-center gap-3 rounded-md border px-3 py-2 text-sm"
                  >
                    {line.status === "ok" && (
                      <CheckCircle2 className="h-4 w-4 shrink-0 text-emerald-500" />
                    )}
                    {line.status === "partial" && (
                      <AlertCircle className="h-4 w-4 shrink-0 text-amber-500" />
                    )}
                    {line.status === "missing" && (
                      <XCircle className="h-4 w-4 shrink-0 text-red-500" />
                    )}
                    <span className="flex-1 font-mono text-xs truncate">
                      {line.partNumber}
                    </span>
                    {line.bomLine.reference && (
                      <span className="text-xs text-muted-foreground truncate max-w-[100px]">
                        {line.bomLine.reference}
                      </span>
                    )}
                    <span className="text-xs tabular-nums">
                      Need: {line.required}
                    </span>
                    <span
                      className={`text-xs tabular-nums ${
                        line.available >= line.required
                          ? "text-emerald-600"
                          : line.available > 0
                            ? "text-amber-600"
                            : "text-red-500"
                      }`}
                    >
                      Have: {line.available}
                    </span>
                  </div>
                ))}
              </div>
            </ScrollArea>

            <div className="flex justify-between">
              <Button variant="outline" onClick={() => setStep("upload")}>
                <ChevronLeft className="mr-1 h-4 w-4" />
                Back
              </Button>
              <Button
                onClick={handlePrepareAllocations}
                disabled={okCount + partialCount === 0}
              >
                Prepare Allocations
                <ChevronRight className="ml-1 h-4 w-4" />
              </Button>
            </div>
          </div>
        )}

        {/* Step 3: Confirm */}
        {step === "confirm" && (
          <div className="space-y-4">
            <p className="text-sm text-muted-foreground">
              The following {allocations.length} takeout operations will be
              executed:
            </p>

            <ScrollArea className="h-[300px]">
              <div className="space-y-1">
                {allocations.map((alloc, i) => (
                  <div
                    key={i}
                    className="flex items-center gap-3 rounded-md border px-3 py-2 text-sm"
                  >
                    <span className="font-mono text-xs truncate flex-1">
                      {alloc.partNumber}
                    </span>
                    <span className="text-xs text-muted-foreground truncate max-w-[200px]">
                      {alloc.locationLabel}
                    </span>
                    <Badge variant="secondary" className="shrink-0">
                      ×{alloc.quantity}
                    </Badge>
                  </div>
                ))}
              </div>
            </ScrollArea>

            {missingCount > 0 && (
              <p className="text-xs text-amber-600 flex items-center gap-1">
                <AlertCircle className="h-3 w-3" />
                {missingCount} line(s) could not be matched and will be skipped.
              </p>
            )}

            <div className="flex justify-between">
              <Button variant="outline" onClick={() => setStep("preview")}>
                <ChevronLeft className="mr-1 h-4 w-4" />
                Back
              </Button>
              <Button onClick={handleExecute}>
                Execute Takeout ({allocations.length} ops)
              </Button>
            </div>
          </div>
        )}

        {/* Step 4: Results */}
        {step === "results" && (
          <div className="space-y-4">
            <div className="h-2 w-full rounded-full bg-muted overflow-hidden">
              <div
                className="h-full rounded-full bg-primary transition-all duration-300"
                style={{ width: `${progress}%` }}
              />
            </div>
            <p className="text-sm text-center text-muted-foreground">
              {progress < 100
                ? `Processing... ${progress}%`
                : "Complete!"}
            </p>

            {progress === 100 && (
              <>
                <div className="flex justify-center gap-3">
                  <Badge variant="secondary" className="gap-1">
                    <CheckCircle2 className="h-3 w-3 text-emerald-500" />
                    {results.success} succeeded
                  </Badge>
                  {results.failed > 0 && (
                    <Badge variant="secondary" className="gap-1">
                      <XCircle className="h-3 w-3 text-red-500" />
                      {results.failed} failed
                    </Badge>
                  )}
                </div>

                {results.errors.length > 0 && (
                  <ScrollArea className="h-[120px]">
                    <div className="space-y-1">
                      {results.errors.map((err, i) => (
                        <p
                          key={i}
                          className="text-xs text-red-500 font-mono"
                        >
                          {err}
                        </p>
                      ))}
                    </div>
                  </ScrollArea>
                )}

                <div className="flex justify-end">
                  <Button
                    onClick={() => {
                      reset();
                      onOpenChange(false);
                    }}
                  >
                    Done
                  </Button>
                </div>
              </>
            )}
          </div>
        )}
      </DialogContent>
    </Dialog>
  );
}
