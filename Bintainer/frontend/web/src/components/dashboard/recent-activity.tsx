"use client";

import { Plus, Minus, RefreshCw } from "lucide-react";
import { cn } from "@/lib/utils";

interface ActivityItem {
  id: string;
  description: string;
  type: "added" | "used" | "restocked";
  timestamp: string;
}

const iconMap = {
  added: { icon: Plus, className: "bg-green-100 text-green-600 dark:bg-green-900/50 dark:text-green-400" },
  used: { icon: Minus, className: "bg-red-100 text-red-600 dark:bg-red-900/50 dark:text-red-400" },
  restocked: { icon: RefreshCw, className: "bg-blue-100 text-blue-600 dark:bg-blue-900/50 dark:text-blue-400" },
};

// Placeholder data until API is connected
const placeholderActivity: ActivityItem[] = [
  { id: "1", description: "STM32F103 added", type: "added", timestamp: "2 min ago" },
  { id: "2", description: "10k resistor used", type: "used", timestamp: "15 min ago" },
  { id: "3", description: "Storage unit Capacitors updated", type: "restocked", timestamp: "1 hour ago" },
  { id: "4", description: "Capacitor 100nF restocked", type: "restocked", timestamp: "3 hours ago" },
];

export function RecentActivity() {
  const activities = placeholderActivity;

  return (
    <div className="rounded-xl border bg-card p-5 shadow-sm">
      <h3 className="mb-4 font-semibold text-card-foreground">Recent Activity</h3>
      {activities.length === 0 ? (
        <p className="text-sm text-muted-foreground">No recent activity</p>
      ) : (
        <div className="space-y-3">
          {activities.map((activity) => {
            const { icon: Icon, className } = iconMap[activity.type];
            return (
              <div key={activity.id} className="flex items-start gap-3">
                <div
                  className={cn(
                    "flex h-7 w-7 shrink-0 items-center justify-center rounded-full",
                    className
                  )}
                >
                  <Icon className="h-3.5 w-3.5" />
                </div>
                <div className="min-w-0 flex-1">
                  <p className="text-sm text-card-foreground">{activity.description}</p>
                  <p className="text-xs text-muted-foreground">{activity.timestamp}</p>
                </div>
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
}
