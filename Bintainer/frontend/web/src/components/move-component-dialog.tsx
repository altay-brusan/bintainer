"use client";

import { useState, useEffect } from "react";
import { ArrowRight } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
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
import type { Component } from "@/lib/demo-data";
import { demoStorageUnits } from "@/lib/demo-data";
import { toast } from "sonner";

interface MoveComponentDialogProps {
  component: Component | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function MoveComponentDialog({
  component,
  open,
  onOpenChange,
}: MoveComponentDialogProps) {
  const [destStorageUnit, setDestStorageUnit] = useState("");
  const [destRow, setDestRow] = useState(1);
  const [destCol, setDestCol] = useState(1);
  const [destCompartment, setDestCompartment] = useState(1);

  // Reset destination when dialog opens
  useEffect(() => {
    if (open && component) {
      setDestStorageUnit("");
      setDestRow(1);
      setDestCol(1);
      setDestCompartment(1);
    }
  }, [open, component]);

  if (!component) return null;

  // Parse current position from component
  const currentStorageUnit = component.storageUnit;
  const currentBin = component.bin; // e.g. "R03-C02"
  const currentRow = parseInt(currentBin.match(/R(\d+)/)?.[1] ?? "0", 10);
  const currentCol = parseInt(currentBin.match(/C(\d+)/)?.[1] ?? "0", 10);
  const currentCompartment = component.compartment; // e.g. "P01"

  const selectedUnit = demoStorageUnits.find((su) => su.name === destStorageUnit);
  const maxRows = selectedUnit?.rows ?? 1;
  const maxCols = selectedUnit?.columns ?? 1;

  const handleMove = () => {
    if (!destStorageUnit) {
      toast.error("Please select a destination storage unit");
      return;
    }
    const destBin = `R${String(destRow).padStart(2, "0")}-C${String(destCol).padStart(2, "0")}`;
    const destComp = `P${String(destCompartment).padStart(2, "0")}`;
    // TODO: call API
    console.log("Moving component:", {
      id: component.id,
      from: { storageUnit: currentStorageUnit, bin: currentBin, compartment: currentCompartment },
      to: { storageUnit: destStorageUnit, bin: destBin, compartment: destComp },
    });
    toast.success(`Moved ${component.name} to ${destStorageUnit} ${destBin} ${destComp} (demo)`);
    onOpenChange(false);
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-lg">
        <DialogHeader>
          <DialogTitle>Move Component</DialogTitle>
        </DialogHeader>

        <div className="space-y-6 py-2">
          {/* Component name */}
          <div className="rounded-lg border bg-muted/50 p-3">
            <p className="text-sm text-muted-foreground">Component</p>
            <p className="font-semibold">{component.name}</p>
          </div>

          <div className="grid grid-cols-[1fr_auto_1fr] items-start gap-4">
            {/* Current Position */}
            <div className="space-y-3">
              <h4 className="text-sm font-semibold text-muted-foreground uppercase tracking-wider">
                Current Position
              </h4>
              <div className="rounded-lg border p-3 space-y-2">
                <div>
                  <p className="text-xs text-muted-foreground">Storage Unit</p>
                  <p className="text-sm font-medium">{currentStorageUnit}</p>
                </div>
                <div className="grid grid-cols-2 gap-2">
                  <div>
                    <p className="text-xs text-muted-foreground">Row</p>
                    <p className="text-sm font-mono font-medium">{currentRow}</p>
                  </div>
                  <div>
                    <p className="text-xs text-muted-foreground">Column</p>
                    <p className="text-sm font-mono font-medium">{currentCol}</p>
                  </div>
                </div>
                <div>
                  <p className="text-xs text-muted-foreground">Compartment</p>
                  <p className="text-sm font-mono font-medium">{currentCompartment}</p>
                </div>
              </div>
            </div>

            {/* Arrow */}
            <div className="flex items-center justify-center pt-16">
              <ArrowRight className="h-6 w-6 text-muted-foreground" />
            </div>

            {/* Destination */}
            <div className="space-y-3">
              <h4 className="text-sm font-semibold text-muted-foreground uppercase tracking-wider">
                Destination
              </h4>
              <div className="rounded-lg border p-3 space-y-3">
                <div>
                  <Label className="text-xs">Storage Unit</Label>
                  <Select value={destStorageUnit} onValueChange={setDestStorageUnit}>
                    <SelectTrigger className="mt-1">
                      <SelectValue placeholder="Select unit" />
                    </SelectTrigger>
                    <SelectContent>
                      {demoStorageUnits.map((su) => (
                        <SelectItem key={su.id} value={su.name}>
                          {su.name}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
                <div className="grid grid-cols-2 gap-2">
                  <div>
                    <Label className="text-xs">Row</Label>
                    <Input
                      type="number"
                      min={1}
                      max={maxRows}
                      value={destRow}
                      onChange={(e) => setDestRow(Math.max(1, Math.min(maxRows, Number(e.target.value) || 1)))}
                      className="mt-1"
                    />
                  </div>
                  <div>
                    <Label className="text-xs">Column</Label>
                    <Input
                      type="number"
                      min={1}
                      max={maxCols}
                      value={destCol}
                      onChange={(e) => setDestCol(Math.max(1, Math.min(maxCols, Number(e.target.value) || 1)))}
                      className="mt-1"
                    />
                  </div>
                </div>
                <div>
                  <Label className="text-xs">Compartment</Label>
                  <Input
                    type="number"
                    min={1}
                    max={5}
                    value={destCompartment}
                    onChange={(e) => setDestCompartment(Math.max(1, Math.min(5, Number(e.target.value) || 1)))}
                    className="mt-1"
                  />
                </div>
              </div>
            </div>
          </div>
        </div>

        {/* Footer */}
        <div className="flex justify-end gap-3 pt-2">
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Cancel
          </Button>
          <Button onClick={handleMove} disabled={!destStorageUnit}>
            <ArrowRight className="mr-2 h-4 w-4" />
            Move Component
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  );
}
