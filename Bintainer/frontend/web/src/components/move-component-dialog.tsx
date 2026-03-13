"use client";

import { useState, useEffect, useMemo } from "react";
import { ArrowRight, Loader2, MapPin } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { Badge } from "@/components/ui/badge";
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
import { useComponent } from "@/hooks/use-components";
import { useInventories } from "@/hooks/use-inventories";
import { useAllStorageUnits, useStorageUnit } from "@/hooks/use-storage-units";
import { useMoveComponent } from "@/hooks/use-component-actions";
import { toast } from "sonner";
import type { ComponentLocationResponse } from "@/types/api";

interface MoveComponentDialogProps {
  component: { id: string; partNumber: string } | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
  /** Pre-select a source compartment (when opened from storage unit page) */
  preselectedSourceCompartmentId?: string;
}

export function MoveComponentDialog({
  component,
  open,
  onOpenChange,
  preselectedSourceCompartmentId,
}: MoveComponentDialogProps) {
  const { data: fullComponent, isLoading: loadingComponent } = useComponent(
    component?.id ?? ""
  );
  const { data: inventories } = useInventories();
  const inventoryIds = (inventories ?? []).map((inv) => inv.id);
  const { data: storageUnits } = useAllStorageUnits(inventoryIds);
  const moveComponent = useMoveComponent();

  const [sourceCompartmentId, setSourceCompartmentId] = useState("");
  const [destStorageUnitId, setDestStorageUnitId] = useState("");
  const [destBinId, setDestBinId] = useState("");
  const [destCompartmentId, setDestCompartmentId] = useState("");
  const [quantity, setQuantity] = useState(1);

  const { data: destStorageUnit } = useStorageUnit(destStorageUnitId);

  const locations = fullComponent?.locations ?? [];
  const units = storageUnits ?? [];

  // Find selected source location
  const sourceLocation = locations.find(
    (l) => l.compartmentId === sourceCompartmentId
  );
  const maxQty = sourceLocation?.quantity ?? 0;

  // Get bins and compartments for destination
  const destBins = destStorageUnit?.bins?.filter((b) => b.isActive) ?? [];
  const destBin = destBins.find((b) => b.id === destBinId);
  const destCompartments =
    destBin?.compartments?.filter((c) => c.isActive) ?? [];

  // Reset state when dialog opens/closes or component changes
  useEffect(() => {
    if (open && component) {
      setSourceCompartmentId(preselectedSourceCompartmentId ?? "");
      setDestStorageUnitId("");
      setDestBinId("");
      setDestCompartmentId("");
      setQuantity(1);
    }
  }, [open, component?.id, preselectedSourceCompartmentId]);

  // Auto-select source if only one location
  useEffect(() => {
    if (locations.length === 1 && !sourceCompartmentId) {
      setSourceCompartmentId(locations[0].compartmentId);
    }
  }, [locations, sourceCompartmentId]);

  // Auto-select preselected source
  useEffect(() => {
    if (preselectedSourceCompartmentId && !sourceCompartmentId) {
      setSourceCompartmentId(preselectedSourceCompartmentId);
    }
  }, [preselectedSourceCompartmentId, sourceCompartmentId]);

  // Reset downstream when upstream changes
  useEffect(() => {
    setDestBinId("");
    setDestCompartmentId("");
  }, [destStorageUnitId]);

  useEffect(() => {
    setDestCompartmentId("");
  }, [destBinId]);

  // Clamp quantity when source changes
  useEffect(() => {
    if (maxQty > 0 && quantity > maxQty) setQuantity(maxQty);
    if (maxQty > 0 && quantity < 1) setQuantity(1);
  }, [maxQty, quantity]);

  const canMove =
    sourceCompartmentId &&
    destCompartmentId &&
    sourceCompartmentId !== destCompartmentId &&
    quantity >= 1 &&
    quantity <= maxQty &&
    !moveComponent.isPending;

  const handleMove = () => {
    if (!component || !canMove) return;
    moveComponent.mutate(
      {
        id: component.id,
        sourceCompartmentId,
        destinationCompartmentId: destCompartmentId,
        quantity,
      },
      {
        onSuccess: () => {
          toast.success(
            `Moved ${quantity}× ${component.partNumber} successfully`
          );
          onOpenChange(false);
        },
        onError: () => {
          toast.error("Failed to move component");
        },
      }
    );
  };

  if (!component) return null;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-lg">
        <DialogHeader>
          <DialogTitle>Move Component</DialogTitle>
        </DialogHeader>

        {loadingComponent ? (
          <div className="flex items-center justify-center py-8">
            <Loader2 className="h-5 w-5 animate-spin text-muted-foreground" />
          </div>
        ) : (
          <div className="space-y-5 py-2">
            {/* Component name */}
            <div className="rounded-lg border bg-muted/50 p-3">
              <p className="text-sm text-muted-foreground">Component</p>
              <p className="font-semibold">{component.partNumber}</p>
            </div>

            {/* Source */}
            <div className="space-y-2">
              <Label className="text-xs font-semibold uppercase tracking-wider text-muted-foreground">
                Source
              </Label>
              {locations.length === 0 ? (
                <p className="text-sm text-muted-foreground italic">
                  This component is not stored anywhere
                </p>
              ) : (
                <Select
                  value={sourceCompartmentId}
                  onValueChange={setSourceCompartmentId}
                >
                  <SelectTrigger>
                    <SelectValue placeholder="Select source location" />
                  </SelectTrigger>
                  <SelectContent>
                    {locations.map((loc) => (
                      <SelectItem
                        key={loc.compartmentId}
                        value={loc.compartmentId}
                      >
                        <span className="flex items-center gap-2">
                          <MapPin className="h-3 w-3" />
                          {loc.storageUnitName} &gt; {loc.label}
                          <Badge variant="secondary" className="ml-1 text-xs">
                            Qty: {loc.quantity}
                          </Badge>
                        </span>
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              )}
            </div>

            {/* Quantity */}
            {sourceCompartmentId && maxQty > 0 && (
              <div className="space-y-2">
                <Label className="text-xs font-semibold uppercase tracking-wider text-muted-foreground">
                  Quantity
                </Label>
                <div className="flex items-center gap-2">
                  <input
                    type="range"
                    min={1}
                    max={maxQty}
                    value={quantity}
                    onChange={(e) => setQuantity(Number(e.target.value))}
                    className="flex-1"
                  />
                  <span className="w-16 text-center font-mono text-sm">
                    {quantity} / {maxQty}
                  </span>
                </div>
              </div>
            )}

            {/* Arrow */}
            {sourceCompartmentId && (
              <div className="flex justify-center">
                <ArrowRight className="h-5 w-5 text-muted-foreground" />
              </div>
            )}

            {/* Destination */}
            {sourceCompartmentId && (
              <div className="space-y-3">
                <Label className="text-xs font-semibold uppercase tracking-wider text-muted-foreground">
                  Destination
                </Label>
                <div className="rounded-lg border p-3 space-y-3">
                  {/* Storage Unit */}
                  <div>
                    <Label className="text-xs">Storage Unit</Label>
                    <Select
                      value={destStorageUnitId}
                      onValueChange={setDestStorageUnitId}
                    >
                      <SelectTrigger className="mt-1">
                        <SelectValue placeholder="Select storage unit" />
                      </SelectTrigger>
                      <SelectContent>
                        {units.map((su) => (
                          <SelectItem key={su.id} value={su.id}>
                            {su.name} ({su.rows}×{su.columns})
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>

                  {/* Bin */}
                  {destStorageUnitId && (
                    <div>
                      <Label className="text-xs">Bin</Label>
                      <Select
                        value={destBinId}
                        onValueChange={setDestBinId}
                      >
                        <SelectTrigger className="mt-1">
                          <SelectValue placeholder="Select bin" />
                        </SelectTrigger>
                        <SelectContent>
                          {destBins.map((bin) => (
                            <SelectItem key={bin.id} value={bin.id}>
                              Row {bin.row + 1}, Col {bin.column + 1}
                              {bin.compartments.some((c) => c.componentId)
                                ? " (has components)"
                                : ""}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                    </div>
                  )}

                  {/* Compartment */}
                  {destBinId && (
                    <div>
                      <Label className="text-xs">Compartment</Label>
                      <Select
                        value={destCompartmentId}
                        onValueChange={setDestCompartmentId}
                      >
                        <SelectTrigger className="mt-1">
                          <SelectValue placeholder="Select compartment" />
                        </SelectTrigger>
                        <SelectContent>
                          {destCompartments.map((comp) => (
                            <SelectItem
                              key={comp.id}
                              value={comp.id}
                              disabled={comp.id === sourceCompartmentId}
                            >
                              {comp.label}
                              {comp.componentPartNumber
                                ? ` — ${comp.componentPartNumber} (${comp.quantity})`
                                : " — Empty"}
                              {comp.id === sourceCompartmentId
                                ? " (source)"
                                : ""}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                    </div>
                  )}
                </div>
              </div>
            )}
          </div>
        )}

        {/* Footer */}
        <div className="flex justify-end gap-3 pt-2">
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Cancel
          </Button>
          <Button onClick={handleMove} disabled={!canMove}>
            {moveComponent.isPending ? (
              <Loader2 className="mr-2 h-4 w-4 animate-spin" />
            ) : (
              <ArrowRight className="mr-2 h-4 w-4" />
            )}
            Move Component
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  );
}
