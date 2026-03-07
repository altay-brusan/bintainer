"use client";

import { useRef, useState } from "react";
import { FileSpreadsheet, Upload, X, CheckCircle2, AlertCircle } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Badge } from "@/components/ui/badge";
import { ScrollArea } from "@/components/ui/scroll-area";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { toast } from "sonner";

interface BomLine {
  partNumber: string;
  quantity: number;
  description: string;
  matched: boolean;
}

interface ImportBomDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function ImportBomDialog({ open, onOpenChange }: ImportBomDialogProps) {
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [file, setFile] = useState<File | null>(null);
  const [parsed, setParsed] = useState(false);
  const [bomLines, setBomLines] = useState<BomLine[]>([]);
  const [partNumberCol, setPartNumberCol] = useState("0");
  const [quantityCol, setQuantityCol] = useState("1");
  const [descriptionCol, setDescriptionCol] = useState("2");

  const handleFileSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
    const f = e.target.files?.[0];
    if (!f) return;
    setFile(f);
    setParsed(false);
    setBomLines([]);
  };

  const handleParse = () => {
    if (!file) return;
    // Demo: generate sample parsed data
    const demoLines: BomLine[] = [
      { partNumber: "RC0603FR-0710KL", quantity: 20, description: "10k Resistor 0603", matched: true },
      { partNumber: "CL05B104KO5NNNC", quantity: 15, description: "100nF Cap 0402", matched: true },
      { partNumber: "STM32F103C8T6", quantity: 5, description: "STM32F103 MCU LQFP-48", matched: true },
      { partNumber: "TPS63020DSJR", quantity: 3, description: "Buck-Boost Converter", matched: false },
      { partNumber: "ESP32-S3-WROOM-1", quantity: 2, description: "ESP32-S3 WiFi Module", matched: true },
      { partNumber: "SN74LVC1G08DBVR", quantity: 10, description: "Single AND Gate", matched: false },
    ];
    setBomLines(demoLines);
    setParsed(true);
  };

  const handleImport = () => {
    const matched = bomLines.filter((l) => l.matched).length;
    const newParts = bomLines.filter((l) => !l.matched).length;
    toast.success(`BOM imported: ${matched} matched, ${newParts} new parts created`);
    resetForm();
    onOpenChange(false);
  };

  const resetForm = () => {
    setFile(null);
    setParsed(false);
    setBomLines([]);
    setPartNumberCol("0");
    setQuantityCol("1");
    setDescriptionCol("2");
    if (fileInputRef.current) fileInputRef.current.value = "";
  };

  const matchedCount = bomLines.filter((l) => l.matched).length;
  const unmatchedCount = bomLines.filter((l) => !l.matched).length;

  return (
    <Dialog open={open} onOpenChange={(v) => { onOpenChange(v); if (!v) resetForm(); }}>
      <DialogContent className="sm:max-w-2xl">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <FileSpreadsheet className="h-5 w-5" />
            Import from Excel / BOM
          </DialogTitle>
        </DialogHeader>

        <div className="space-y-5">
          {/* File upload */}
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
                    onClick={(e) => { e.stopPropagation(); setFile(null); setParsed(false); setBomLines([]); }}
                  >
                    <X className="h-4 w-4" />
                  </Button>
                </div>
              ) : (
                <div className="flex flex-col items-center gap-2 text-muted-foreground">
                  <Upload className="h-8 w-8" />
                  <p className="text-sm">Click to upload CSV, XLSX, or XLS file</p>
                  <p className="text-xs">Supports standard BOM formats</p>
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

          {/* Column mapping */}
          {file && !parsed && (
            <div className="space-y-3">
              <Label className="text-sm font-semibold">Column Mapping</Label>
              <p className="text-xs text-muted-foreground">
                Map your BOM columns to the required fields
              </p>
              <div className="grid grid-cols-3 gap-3">
                <div>
                  <Label className="text-xs">Part Number Column</Label>
                  <Select value={partNumberCol} onValueChange={setPartNumberCol}>
                    <SelectTrigger className="mt-1">
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      {["A", "B", "C", "D", "E", "F"].map((c, i) => (
                        <SelectItem key={c} value={String(i)}>Column {c}</SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
                <div>
                  <Label className="text-xs">Quantity Column</Label>
                  <Select value={quantityCol} onValueChange={setQuantityCol}>
                    <SelectTrigger className="mt-1">
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      {["A", "B", "C", "D", "E", "F"].map((c, i) => (
                        <SelectItem key={c} value={String(i)}>Column {c}</SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
                <div>
                  <Label className="text-xs">Description Column</Label>
                  <Select value={descriptionCol} onValueChange={setDescriptionCol}>
                    <SelectTrigger className="mt-1">
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      {["A", "B", "C", "D", "E", "F"].map((c, i) => (
                        <SelectItem key={c} value={String(i)}>Column {c}</SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
              </div>
              <Button onClick={handleParse} className="w-full">
                Parse BOM
              </Button>
            </div>
          )}

          {/* Parsed results */}
          {parsed && (
            <div className="space-y-3">
              <div className="flex items-center gap-3">
                <Badge variant="secondary" className="gap-1">
                  <CheckCircle2 className="h-3 w-3 text-emerald-500" />
                  {matchedCount} matched
                </Badge>
                <Badge variant="secondary" className="gap-1">
                  <AlertCircle className="h-3 w-3 text-amber-500" />
                  {unmatchedCount} new
                </Badge>
                <span className="text-xs text-muted-foreground">
                  {bomLines.length} total lines
                </span>
              </div>

              <ScrollArea className="h-[280px] rounded-lg border">
                <div className="p-1">
                  <div className="grid grid-cols-[1fr_80px_1fr_70px] gap-2 px-3 py-2 text-xs font-medium text-muted-foreground border-b bg-muted/50">
                    <span>Part Number</span>
                    <span>Qty</span>
                    <span>Description</span>
                    <span>Status</span>
                  </div>
                  {bomLines.map((line, i) => (
                    <div key={i} className="grid grid-cols-[1fr_80px_1fr_70px] gap-2 items-center px-3 py-2 text-sm border-b last:border-0">
                      <span className="font-mono text-xs">{line.partNumber}</span>
                      <span>{line.quantity}</span>
                      <span className="text-muted-foreground truncate text-xs">{line.description}</span>
                      <span>
                        {line.matched ? (
                          <Badge variant="secondary" className="text-[10px] gap-0.5 text-emerald-600">
                            <CheckCircle2 className="h-2.5 w-2.5" /> Found
                          </Badge>
                        ) : (
                          <Badge variant="secondary" className="text-[10px] gap-0.5 text-amber-600">
                            <AlertCircle className="h-2.5 w-2.5" /> New
                          </Badge>
                        )}
                      </span>
                    </div>
                  ))}
                </div>
              </ScrollArea>
            </div>
          )}
        </div>

        <div className="flex justify-end gap-3 pt-2">
          <Button variant="outline" onClick={() => { resetForm(); onOpenChange(false); }}>
            Cancel
          </Button>
          <Button onClick={handleImport} disabled={!parsed || bomLines.length === 0}>
            Import {bomLines.length} Components
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  );
}
