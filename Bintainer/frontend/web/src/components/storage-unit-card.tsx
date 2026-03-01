"use client";

import Link from "next/link";
import { Grid3X3 } from "lucide-react";
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
} from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import type { StorageUnitSummary } from "@/types/api";

interface StorageUnitCardProps {
  storageUnit: StorageUnitSummary;
}

export function StorageUnitCard({ storageUnit }: StorageUnitCardProps) {
  return (
    <Link href={`/storage-units/${storageUnit.id}`}>
      <Card className="transition-colors hover:bg-accent/50 cursor-pointer">
        <CardHeader>
          <div className="flex items-center gap-3">
            <div className="rounded-md bg-primary/10 p-2">
              <Grid3X3 className="h-5 w-5 text-primary" />
            </div>
            <div className="flex-1">
              <CardTitle className="text-lg">{storageUnit.name}</CardTitle>
              <CardDescription>
                {storageUnit.columns} &times; {storageUnit.rows} grid
              </CardDescription>
            </div>
            <Badge variant="secondary">
              {storageUnit.compartmentCount} compartments
            </Badge>
          </div>
        </CardHeader>
      </Card>
    </Link>
  );
}
