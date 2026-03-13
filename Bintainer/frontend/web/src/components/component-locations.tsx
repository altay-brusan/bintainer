"use client";

import { useState } from "react";
import { MapPin, Minus, Plus, Eye, EyeOff, Loader2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { useAdjustQuantity } from "@/hooks/use-component-actions";
import { LocationBinViewer } from "@/components/location-bin-viewer";
import { toast } from "sonner";
import type { ComponentLocationResponse } from "@/types/api";

interface ComponentLocationsProps {
  componentId: string;
  partNumber: string;
  locations: ComponentLocationResponse[];
  onTakeOutComplete?: () => void;
}

export function ComponentLocations({
  componentId,
  partNumber,
  locations,
  onTakeOutComplete,
}: ComponentLocationsProps) {
  const [customTakeoutLocation, setCustomTakeoutLocation] = useState<string | null>(null);
  const [customQty, setCustomQty] = useState(1);
  const [viewerLocation, setViewerLocation] = useState<string | null>(null);
  const [pendingCompartment, setPendingCompartment] = useState<string | null>(null);
  const adjustQuantity = useAdjustQuantity();

  if (locations.length === 0) {
    return (
      <div className="rounded-lg border border-dashed p-4 text-center text-sm text-muted-foreground">
        <MapPin className="mx-auto mb-1 h-4 w-4" />
        Not stored in any location
      </div>
    );
  }

  const doTakeOut = (loc: ComponentLocationResponse, qty: number) => {
    const amount = Math.min(qty, loc.quantity);
    if (amount <= 0) return;
    setPendingCompartment(loc.compartmentId);
    adjustQuantity.mutate(
      {
        id: componentId,
        compartmentId: loc.compartmentId,
        action: "Used",
        quantity: amount,
        notes: `Taken out from ${loc.storageUnitName} > ${loc.label}`,
      },
      {
        onSuccess: () => {
          toast.success(
            `Took out ${amount}× ${partNumber} from ${loc.storageUnitName} > ${loc.label}`
          );
          setPendingCompartment(null);
          setCustomTakeoutLocation(null);
          setCustomQty(1);
          onTakeOutComplete?.();
        },
        onError: () => {
          toast.error("Failed to take out component");
          setPendingCompartment(null);
        },
      }
    );
  };

  return (
    <div className="space-y-2">
      <p className="text-xs font-semibold text-muted-foreground uppercase tracking-wide">
        Locations ({locations.length})
      </p>
      <div className="space-y-2">
        {locations.map((loc) => {
          const isCustom = customTakeoutLocation === loc.compartmentId;
          const isViewing = viewerLocation === loc.compartmentId;
          const isPending = pendingCompartment === loc.compartmentId;
          const maxQty = loc.quantity;

          return (
            <div key={loc.compartmentId} className="space-y-2">
              <div className="flex items-center gap-2 rounded-lg border bg-card p-3">
                <MapPin className="h-4 w-4 shrink-0 text-muted-foreground" />
                <div className="flex-1 min-w-0">
                  <p className="text-sm font-medium truncate">
                    {loc.storageUnitName}
                    <span className="text-muted-foreground"> &gt; </span>
                    {loc.label}
                  </p>
                </div>
                <Badge variant="secondary" className="shrink-0">
                  Qty: {loc.quantity}
                </Badge>
                <Button
                  variant="ghost"
                  size="sm"
                  className="h-7 gap-1 text-xs"
                  onClick={() =>
                    setViewerLocation(isViewing ? null : loc.compartmentId)
                  }
                >
                  {isViewing ? (
                    <EyeOff className="h-3 w-3" />
                  ) : (
                    <Eye className="h-3 w-3" />
                  )}
                  Position
                </Button>
                {/* Single-click: take out 1 */}
                <Button
                  variant="outline"
                  size="sm"
                  className="h-7 text-xs"
                  disabled={loc.quantity === 0 || isPending}
                  onClick={() => doTakeOut(loc, 1)}
                >
                  {isPending ? (
                    <Loader2 className="h-3 w-3 animate-spin" />
                  ) : (
                    "Take 1"
                  )}
                </Button>
                {/* Toggle custom qty stepper */}
                {loc.quantity > 1 && (
                  <Button
                    variant={isCustom ? "secondary" : "ghost"}
                    size="sm"
                    className="h-7 text-xs"
                    onClick={() => {
                      setCustomTakeoutLocation(isCustom ? null : loc.compartmentId);
                      setCustomQty(1);
                    }}
                  >
                    More...
                  </Button>
                )}
              </div>

              {/* Custom quantity stepper */}
              {isCustom && (
                <div className="ml-6 flex items-center gap-2 rounded-lg border border-dashed bg-muted/30 p-2">
                  <span className="text-xs text-muted-foreground">Qty:</span>
                  <div className="flex items-center gap-1">
                    <Button
                      variant="outline"
                      size="icon"
                      className="h-6 w-6"
                      onClick={() => setCustomQty(Math.max(1, customQty - 1))}
                      disabled={customQty <= 1}
                    >
                      <Minus className="h-3 w-3" />
                    </Button>
                    <span className="w-8 text-center text-sm font-mono">
                      {customQty}
                    </span>
                    <Button
                      variant="outline"
                      size="icon"
                      className="h-6 w-6"
                      onClick={() =>
                        setCustomQty(Math.min(maxQty, customQty + 1))
                      }
                      disabled={customQty >= maxQty}
                    >
                      <Plus className="h-3 w-3" />
                    </Button>
                  </div>
                  <span className="text-xs text-muted-foreground">
                    of {maxQty}
                  </span>
                  <div className="ml-auto flex gap-1">
                    <Button
                      variant="ghost"
                      size="sm"
                      className="h-6 text-xs"
                      onClick={() => setCustomTakeoutLocation(null)}
                    >
                      Cancel
                    </Button>
                    <Button
                      size="sm"
                      className="h-6 text-xs"
                      onClick={() => doTakeOut(loc, customQty)}
                      disabled={isPending || customQty < 1 || maxQty === 0}
                    >
                      {isPending ? "..." : `Take ${customQty}`}
                    </Button>
                  </div>
                </div>
              )}

              {/* Bin viewer */}
              {isViewing && (
                <div className="ml-6">
                  <LocationBinViewer
                    storageUnitId={loc.storageUnitId}
                    targetBinId={loc.binId}
                  />
                </div>
              )}
            </div>
          );
        })}
      </div>

      {/* Multi-bin hint */}
      {locations.length > 1 && (
        <p className="text-xs text-muted-foreground italic">
          This component is stored in {locations.length} locations. Take out from
          each as needed.
        </p>
      )}
    </div>
  );
}
