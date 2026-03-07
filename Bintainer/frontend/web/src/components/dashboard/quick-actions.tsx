"use client";

import { Plus, Archive, Search, FileUp } from "lucide-react";
import { Button } from "@/components/ui/button";
import Link from "next/link";

const actions = [
  { label: "Add Component", icon: Plus, href: "/components?action=add" },
  { label: "Add Storage Unit", icon: Archive, href: "/storage-units?action=add" },
  { label: "Find Component", icon: Search, href: "/find" },
  { label: "Import BOM", icon: FileUp, href: "/components?action=import" },
];

export function QuickActions() {
  return (
    <div className="flex flex-wrap gap-3">
      {actions.map((action) => (
        <Button
          key={action.label}
          variant="outline"
          className="gap-2 bg-card"
          asChild
        >
          <Link href={action.href}>
            <action.icon className="h-4 w-4" />
            {action.label}
          </Link>
        </Button>
      ))}
    </div>
  );
}
