"use client";

import { AlertTriangle, TrendingUp, Archive, Package } from "lucide-react";
import { cn } from "@/lib/utils";
import { demoComponents, demoStorageUnits, demoMovements } from "@/lib/demo-data";

const lowStockComponents = demoComponents.filter(
  (c) => c.quantity <= c.lowStockThreshold
);

const mostUsed = demoMovements
  .filter((m) => m.action === "used")
  .reduce<Record<string, number>>((acc, m) => {
    acc[m.component] = (acc[m.component] || 0) + Math.abs(m.quantity);
    return acc;
  }, {});

const mostUsedSorted = Object.entries(mostUsed)
  .sort(([, a], [, b]) => b - a)
  .slice(0, 5);

const totalComponents = demoComponents.reduce((sum, c) => sum + c.quantity, 0);

export default function ReportsPage() {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold">Reports</h1>
        <p className="text-muted-foreground">Inventory analytics and insights</p>
      </div>

      {/* Overview Cards */}
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <ReportCard
          title="Total Components"
          value={totalComponents.toLocaleString()}
          subtitle={`${demoComponents.length} unique items`}
          icon={Package}
          iconClassName="bg-primary/10 text-primary"
        />
        <ReportCard
          title="Storage Units"
          value={demoStorageUnits.length.toString()}
          subtitle={`${demoStorageUnits.reduce((s, u) => s + u.rows * u.columns, 0)} total bins`}
          icon={Archive}
          iconClassName="bg-emerald-100 text-emerald-600 dark:bg-emerald-900/50 dark:text-emerald-400"
        />
        <ReportCard
          title="Low Stock Items"
          value={lowStockComponents.length.toString()}
          subtitle="Need restocking"
          icon={AlertTriangle}
          iconClassName="bg-red-100 text-red-600 dark:bg-red-900/50 dark:text-red-400"
        />
        <ReportCard
          title="Recent Movements"
          value={demoMovements.length.toString()}
          subtitle="Last 7 days"
          icon={TrendingUp}
          iconClassName="bg-amber-100 text-amber-600 dark:bg-amber-900/50 dark:text-amber-400"
        />
      </div>

      <div className="grid gap-6 lg:grid-cols-2">
        {/* Low Stock */}
        <div className="rounded-xl border bg-card p-5 shadow-sm">
          <h3 className="mb-4 flex items-center gap-2 font-semibold">
            <AlertTriangle className="h-4 w-4 text-destructive" />
            Low Stock Components
          </h3>
          {lowStockComponents.length === 0 ? (
            <p className="text-sm text-muted-foreground">All stock levels are healthy.</p>
          ) : (
            <div className="space-y-3">
              {lowStockComponents.map((comp) => (
                <div key={comp.id} className="flex items-center justify-between rounded-lg border bg-background p-3">
                  <div>
                    <p className="font-medium">{comp.name}</p>
                    <p className="text-xs text-muted-foreground">
                      {comp.storageUnit} &middot; {comp.bin}
                    </p>
                  </div>
                  <div className="text-right">
                    <p className="font-semibold text-destructive">{comp.quantity}</p>
                    <p className="text-xs text-muted-foreground">
                      min: {comp.lowStockThreshold}
                    </p>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>

        {/* Most Used */}
        <div className="rounded-xl border bg-card p-5 shadow-sm">
          <h3 className="mb-4 flex items-center gap-2 font-semibold">
            <TrendingUp className="h-4 w-4 text-primary" />
            Most Used Components
          </h3>
          {mostUsedSorted.length === 0 ? (
            <p className="text-sm text-muted-foreground">No usage data yet.</p>
          ) : (
            <div className="space-y-3">
              {mostUsedSorted.map(([name, count], i) => (
                <div key={name} className="flex items-center gap-3">
                  <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-primary/10 text-sm font-bold text-primary">
                    {i + 1}
                  </div>
                  <div className="flex-1">
                    <p className="font-medium">{name}</p>
                    <div className="mt-1 h-2 rounded-full bg-muted">
                      <div
                        className="h-2 rounded-full bg-primary"
                        style={{
                          width: `${(count / (mostUsedSorted[0]?.[1] || 1)) * 100}%`,
                        }}
                      />
                    </div>
                  </div>
                  <span className="text-sm font-medium text-muted-foreground">
                    {count} used
                  </span>
                </div>
              ))}
            </div>
          )}
        </div>

        {/* Storage Utilization */}
        <div className="rounded-xl border bg-card p-5 shadow-sm lg:col-span-2">
          <h3 className="mb-4 flex items-center gap-2 font-semibold">
            <Archive className="h-4 w-4 text-emerald-600 dark:text-emerald-400" />
            Storage Utilization
          </h3>
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-5">
            {demoStorageUnits.map((su) => {
              const totalBins = su.rows * su.columns;
              const occupiedBins = demoComponents.filter((c) => c.storageUnit === su.name).length;
              const utilization = totalBins > 0 ? Math.round((occupiedBins / totalBins) * 100) : 0;
              return (
                <div key={su.id} className="rounded-lg border bg-background p-3 text-center">
                  <p className="font-medium">{su.name}</p>
                  <p className="mt-2 text-2xl font-bold text-primary">{utilization}%</p>
                  <p className="text-xs text-muted-foreground">
                    {occupiedBins}/{totalBins} bins used
                  </p>
                  <div className="mx-auto mt-2 h-1.5 w-full rounded-full bg-muted">
                    <div
                      className="h-1.5 rounded-full bg-primary"
                      style={{ width: `${utilization}%` }}
                    />
                  </div>
                </div>
              );
            })}
          </div>
        </div>
      </div>
    </div>
  );
}

function ReportCard({
  title,
  value,
  subtitle,
  icon: Icon,
  iconClassName,
}: {
  title: string;
  value: string;
  subtitle: string;
  icon: React.ComponentType<{ className?: string }>;
  iconClassName?: string;
}) {
  return (
    <div className="rounded-xl border bg-card p-5 shadow-sm">
      <div className="flex items-center gap-3">
        <div className={cn("flex h-10 w-10 items-center justify-center rounded-lg", iconClassName)}>
          <Icon className="h-5 w-5" />
        </div>
        <div>
          <p className="text-2xl font-bold">{value}</p>
          <p className="text-sm text-muted-foreground">{title}</p>
        </div>
      </div>
      <p className="mt-2 text-xs text-muted-foreground">{subtitle}</p>
    </div>
  );
}
