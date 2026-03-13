"use client";

import { PackageMinus, Loader2 } from "lucide-react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { useComponent } from "@/hooks/use-components";
import { ComponentLocations } from "@/components/component-locations";

interface TakeoutComponentDialogProps {
  component: { id: string; partNumber: string } | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function TakeoutComponentDialog({
  component,
  open,
  onOpenChange,
}: TakeoutComponentDialogProps) {
  const { data: full, isLoading } = useComponent(component?.id ?? "");

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-lg">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <PackageMinus className="h-5 w-5" />
            Take Out — {component?.partNumber}
          </DialogTitle>
        </DialogHeader>

        {isLoading ? (
          <div className="flex items-center justify-center py-8">
            <Loader2 className="h-5 w-5 animate-spin text-muted-foreground" />
          </div>
        ) : full ? (
          <ComponentLocations
            componentId={full.id}
            partNumber={full.partNumber}
            locations={full.locations}
            onTakeOutComplete={() => {}}
          />
        ) : (
          <p className="py-4 text-center text-sm text-muted-foreground">
            Component not found
          </p>
        )}
      </DialogContent>
    </Dialog>
  );
}
