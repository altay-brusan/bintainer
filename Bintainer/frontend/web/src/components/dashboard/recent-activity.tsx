"use client";

import { Plus, Minus, RefreshCw } from "lucide-react";
import { cn } from "@/lib/utils";
import { useActivityLog } from "@/hooks/use-activity-log";
import type { ActivityLogItemResponse } from "@/types/api";

type ActivityType = "added" | "used" | "restocked";

const iconMap = {
  added: { icon: Plus, className: "bg-green-100 text-green-600 dark:bg-green-900/50 dark:text-green-400" },
  used: { icon: Minus, className: "bg-red-100 text-red-600 dark:bg-red-900/50 dark:text-red-400" },
  restocked: { icon: RefreshCw, className: "bg-blue-100 text-blue-600 dark:bg-blue-900/50 dark:text-blue-400" },
};

function formatRelativeTime(timestamp: string) {
  const diff = Date.now() - new Date(timestamp).getTime();
  const mins = Math.floor(diff / 60000);
  if (mins < 1) return "Just now";
  if (mins < 60) return `${mins} min ago`;
  const hours = Math.floor(mins / 60);
  if (hours < 24) return `${hours} hours ago`;
  const days = Math.floor(hours / 24);
  return `${days} days ago`;
}

function getActivityType(action: string): ActivityType {
  if (/Created|Added/i.test(action)) return "added";
  if (/Deleted|Used/i.test(action)) return "used";
  return "restocked";
}

export function RecentActivity() {
  const { data } = useActivityLog({ page: 1, pageSize: 5 });
  const activities = data?.items ?? [];

  return (
    <div className="rounded-xl border bg-card p-5 shadow-sm">
      <h3 className="mb-4 font-semibold text-card-foreground">Recent Activity</h3>
      {activities.length === 0 ? (
        <p className="text-sm text-muted-foreground">No recent activity</p>
      ) : (
        <div className="space-y-3">
          {activities.map((item: ActivityLogItemResponse) => {
            const type = getActivityType(item.action);
            const { icon: Icon, className } = iconMap[type];
            return (
              <div key={item.id} className="flex items-start gap-3">
                <div
                  className={cn(
                    "flex h-7 w-7 shrink-0 items-center justify-center rounded-full",
                    className
                  )}
                >
                  <Icon className="h-3.5 w-3.5" />
                </div>
                <div className="min-w-0 flex-1">
                  <p className="text-sm text-card-foreground">{item.message ?? item.action}</p>
                  <p className="text-xs text-muted-foreground">{formatRelativeTime(item.timestamp)}</p>
                </div>
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
}
