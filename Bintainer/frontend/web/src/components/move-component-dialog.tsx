"use client";

import { useState, useEffect } from "react";
import { ArrowRight, Info } from "lucide-react";
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
import { useInventories } from "@/hooks/use-inventories";
import { useAllStorageUnits } from "@/hooks/use-storage-units";
import { toast } from "sonner";

interface MoveComponentDialogProps {
  component: { id: string; partNumber: string } | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function MoveComponentDialog({
  component,
  open,
  onOpenChange,
}: MoveComponentDialogProps) {
  const { data: inventories } = useInventories();
  const inventoryIds = (inventories ?? []).map((inv) => inv.id);
  const { data: storageUnits } = useAllStorageUnits(inventoryIds);

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

  const units = storageUnits ?? [];
  const selectedUnit = units.find((su) => su.id === destStorageUnit);
  const maxRows = selectedUnit?.rows ?? 1;
  const maxCols = selectedUnit?.columns ?? 1;

  const handleMove = () => {
    toast.info("Moving components requires source and destination compartment IDs. Please use the storage unit view to move components between specific compartments.");
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
            <p className="font-semibold">{component.partNumber}</p>
          </div>

          {/* Info banner */}
          <div className="flex items-start gap-2 rounded-lg border border-blue-200 bg-blue-50 p-3 dark:border-blue-900 dark:bg-blue-950/50">
            <Info className="h-4 w-4 mt-0.5 text-blue-600 dark:text-blue-400 shrink-0" />
            <p className="text-sm text-blue-700 dark:text-blue-300">
              To move a component, please use the storage unit view where you can select source and destination compartments directly.
            </p>
          </div>

          {/* Destination selection (informational) */}
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
                    {units.map((su) => (
                      <SelectItem key={su.id} value={su.id}>
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

        {/* Footer */}
        <div className="flex justify-end gap-3 pt-2">
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Cancel
          </Button>
          <Button onClick={handleMove} disabled>
            <ArrowRight className="mr-2 h-4 w-4" />
            Move Component
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  );
}
