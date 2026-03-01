"use client";

import Link from "next/link";
import { Package } from "lucide-react";
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
} from "@/components/ui/card";
import type { Inventory } from "@/types/api";

interface InventoryCardProps {
  inventory: Inventory;
}

export function InventoryCard({ inventory }: InventoryCardProps) {
  return (
    <Link href={`/inventories/${inventory.id}`}>
      <Card className="transition-colors hover:bg-accent/50 cursor-pointer">
        <CardHeader>
          <div className="flex items-center gap-3">
            <div className="rounded-md bg-primary/10 p-2">
              <Package className="h-5 w-5 text-primary" />
            </div>
            <div>
              <CardTitle className="text-lg">{inventory.name}</CardTitle>
              <CardDescription>Click to view storage units</CardDescription>
            </div>
          </div>
        </CardHeader>
      </Card>
    </Link>
  );
}
