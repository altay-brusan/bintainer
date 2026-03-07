"use client";

import { useState } from "react";
import { Globe, Search, CheckCircle2, ExternalLink } from "lucide-react";
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

interface SupplierResult {
  partNumber: string;
  mfrPartNumber: string;
  description: string;
  manufacturer: string;
  unitPrice: number;
  stock: number;
  selected: boolean;
}

interface ImportSupplierDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function ImportSupplierDialog({ open, onOpenChange }: ImportSupplierDialogProps) {
  const [supplier, setSupplier] = useState("digikey");
  const [searchQuery, setSearchQuery] = useState("");
  const [searched, setSearched] = useState(false);
  const [results, setResults] = useState<SupplierResult[]>([]);

  const handleSearch = () => {
    if (!searchQuery.trim()) return;
    // Demo results
    const demoResults: SupplierResult[] = [
      { partNumber: "311-10.0KHRCT-ND", mfrPartNumber: "RC0603FR-0710KL", description: "RES 10K OHM 1% 1/10W 0603", manufacturer: "Yageo", unitPrice: 0.01, stock: 45000, selected: false },
      { partNumber: "311-10KGRCT-ND", mfrPartNumber: "RC0603JR-0710KL", description: "RES 10K OHM 5% 1/10W 0603", manufacturer: "Yageo", unitPrice: 0.008, stock: 120000, selected: false },
      { partNumber: "RMCF0603FT10K0CT-ND", mfrPartNumber: "RMCF0603FT10K0", description: "RES 10K OHM 1% 1/10W 0603", manufacturer: "Stackpole", unitPrice: 0.012, stock: 85000, selected: false },
      { partNumber: "P10KGCT-ND", mfrPartNumber: "ERJ-3GEYJ103V", description: "RES 10K OHM 5% 1/10W 0603", manufacturer: "Panasonic", unitPrice: 0.009, stock: 200000, selected: false },
    ];
    setResults(demoResults);
    setSearched(true);
  };

  const toggleResult = (index: number) => {
    setResults((prev) =>
      prev.map((r, i) => (i === index ? { ...r, selected: !r.selected } : r))
    );
  };

  const handleImport = () => {
    const selected = results.filter((r) => r.selected);
    if (selected.length === 0) {
      toast.error("No components selected");
      return;
    }
    toast.success(`${selected.length} component(s) imported from ${supplier === "digikey" ? "DigiKey" : supplier === "mouser" ? "Mouser" : "LCSC"}`);
    resetForm();
    onOpenChange(false);
  };

  const resetForm = () => {
    setSearchQuery("");
    setSearched(false);
    setResults([]);
  };

  const selectedCount = results.filter((r) => r.selected).length;
  const supplierName = supplier === "digikey" ? "DigiKey" : supplier === "mouser" ? "Mouser" : "LCSC";

  return (
    <Dialog open={open} onOpenChange={(v) => { onOpenChange(v); if (!v) resetForm(); }}>
      <DialogContent className="sm:max-w-2xl">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <Globe className="h-5 w-5" />
            Import from Supplier
          </DialogTitle>
        </DialogHeader>

        <div className="space-y-5">
          {/* Supplier selection */}
          <div className="grid grid-cols-2 gap-3">
            <div>
              <Label className="text-xs">Supplier</Label>
              <Select value={supplier} onValueChange={(v) => { setSupplier(v); setSearched(false); setResults([]); }}>
                <SelectTrigger className="mt-1">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="digikey">DigiKey</SelectItem>
                  <SelectItem value="mouser">Mouser</SelectItem>
                  <SelectItem value="lcsc">LCSC</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div>
              <Label className="text-xs">Search {supplierName}</Label>
              <div className="flex gap-2 mt-1">
                <div className="relative flex-1">
                  <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
                  <Input
                    placeholder="Part number, keyword..."
                    value={searchQuery}
                    onChange={(e) => setSearchQuery(e.target.value)}
                    className="pl-9"
                    onKeyDown={(e) => { if (e.key === "Enter") handleSearch(); }}
                  />
                </div>
                <Button onClick={handleSearch} disabled={!searchQuery.trim()}>
                  Search
                </Button>
              </div>
            </div>
          </div>

          {/* Results */}
          {searched && (
            <div className="space-y-3">
              <div className="flex items-center justify-between">
                <span className="text-sm text-muted-foreground">
                  {results.length} results from {supplierName}
                </span>
                {selectedCount > 0 && (
                  <Badge variant="default">{selectedCount} selected</Badge>
                )}
              </div>

              <ScrollArea className="h-[300px] rounded-lg border">
                <div className="p-1">
                  {results.map((result, i) => (
                    <div
                      key={i}
                      className={`flex items-start gap-3 p-3 border-b last:border-0 cursor-pointer transition-colors ${
                        result.selected ? "bg-primary/5" : "hover:bg-muted/50"
                      }`}
                      onClick={() => toggleResult(i)}
                    >
                      <div className={`mt-0.5 flex h-5 w-5 shrink-0 items-center justify-center rounded border ${
                        result.selected ? "border-primary bg-primary text-primary-foreground" : "border-muted-foreground/30"
                      }`}>
                        {result.selected && <CheckCircle2 className="h-3.5 w-3.5" />}
                      </div>
                      <div className="flex-1 min-w-0">
                        <div className="flex items-center gap-2">
                          <span className="font-mono text-sm font-medium">{result.mfrPartNumber}</span>
                          <Badge variant="outline" className="text-[10px]">{result.manufacturer}</Badge>
                        </div>
                        <p className="text-xs text-muted-foreground mt-0.5 truncate">{result.description}</p>
                        <div className="flex items-center gap-3 mt-1 text-xs">
                          <span className="font-mono text-muted-foreground">{result.partNumber}</span>
                          <ExternalLink className="h-3 w-3 text-muted-foreground/50" />
                        </div>
                      </div>
                      <div className="text-right shrink-0">
                        <p className="font-semibold text-sm">${result.unitPrice.toFixed(3)}</p>
                        <p className="text-xs text-muted-foreground">{result.stock.toLocaleString()} in stock</p>
                      </div>
                    </div>
                  ))}
                </div>
              </ScrollArea>
            </div>
          )}

          {!searched && (
            <div className="rounded-lg border border-dashed p-8 text-center text-muted-foreground">
              <Globe className="mx-auto h-8 w-8 mb-2 opacity-30" />
              <p className="text-sm">Search for components from {supplierName}</p>
              <p className="text-xs mt-1">Enter a part number or keyword to get started</p>
            </div>
          )}
        </div>

        <div className="flex justify-end gap-3 pt-2">
          <Button variant="outline" onClick={() => { resetForm(); onOpenChange(false); }}>
            Cancel
          </Button>
          <Button onClick={handleImport} disabled={selectedCount === 0}>
            Import {selectedCount > 0 ? `${selectedCount} Component${selectedCount > 1 ? "s" : ""}` : "Selected"}
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  );
}
