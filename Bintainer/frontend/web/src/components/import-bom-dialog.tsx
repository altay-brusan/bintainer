"use client";

import { useRef, useState } from "react";
import { FileSpreadsheet, Upload, X, CheckCircle2, AlertCircle, Loader2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { Badge } from "@/components/ui/badge";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { useImportBom } from "@/hooks/use-bom";
import { toast } from "sonner";

interface ImportBomDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function ImportBomDialog({ open, onOpenChange }: ImportBomDialogProps) {
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [file, setFile] = useState<File | null>(null);
  const importBom = useImportBom();

  const handleFileSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
    const f = e.target.files?.[0];
    if (!f) return;
    setFile(f);
  };

  const handleImport = () => {
    if (!file) return;

    importBom.mutate(file, {
      onSuccess: (result) => {
        toast.success(
          `BOM imported: ${result.matchedCount} matched, ${result.newCount} new parts created (${result.totalLines} lines)`
        );
        resetForm();
        onOpenChange(false);
      },
      onError: () => {
        toast.error("Failed to import BOM file");
      },
    });
  };

  const resetForm = () => {
    setFile(null);
    if (fileInputRef.current) fileInputRef.current.value = "";
  };

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
                    onClick={(e) => { e.stopPropagation(); setFile(null); }}
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

          {/* Import result display */}
          {importBom.isSuccess && importBom.data && (
            <div className="space-y-3">
              <div className="flex items-center gap-3">
                <Badge variant="secondary" className="gap-1">
                  <CheckCircle2 className="h-3 w-3 text-emerald-500" />
                  {importBom.data.matchedCount} matched
                </Badge>
                <Badge variant="secondary" className="gap-1">
                  <AlertCircle className="h-3 w-3 text-amber-500" />
                  {importBom.data.newCount} new
                </Badge>
                <span className="text-xs text-muted-foreground">
                  {importBom.data.totalLines} total lines
                </span>
              </div>
            </div>
          )}
        </div>

        <div className="flex justify-end gap-3 pt-2">
          <Button variant="outline" onClick={() => { resetForm(); onOpenChange(false); }}>
            Cancel
          </Button>
          <Button
            onClick={handleImport}
            disabled={!file || importBom.isPending}
          >
            {importBom.isPending && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
            Import BOM
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  );
}
