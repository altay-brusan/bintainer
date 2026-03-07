"use client";

import {
  Cpu,
  Archive,
  Grid3x3,
  Layers,
  AlertTriangle,
} from "lucide-react";
import { SummaryCard } from "@/components/dashboard/summary-card";
import { QuickActions } from "@/components/dashboard/quick-actions";
import { StorageUnitPreview } from "@/components/dashboard/storage-unit-preview";
import { StorageUnitDetailCard } from "@/components/dashboard/storage-unit-detail-card";
import { RecentActivity } from "@/components/dashboard/recent-activity";
import { demoStorageUnits, demoComponents } from "@/lib/demo-data";

const colorSchemes = ["blue", "amber", "emerald", "red", "purple"] as const;

export default function DashboardPage() {
  const storageUnits = demoStorageUnits;
  const totalBins = storageUnits.reduce((sum, su) => sum + su.rows * su.columns, 0);
  const totalComponents = demoComponents.reduce((sum, c) => sum + c.quantity, 0);
  const lowStock = demoComponents.filter((c) => c.quantity <= c.lowStockThreshold).length;

  return (
    <div className="space-y-8">
      <div>
        <h1 className="text-2xl font-bold">Dashboard</h1>
        <p className="text-muted-foreground">Overview of your inventory</p>
      </div>

      {/* Summary Cards */}
      <div className="grid grid-cols-2 gap-4 lg:grid-cols-5">
        <SummaryCard
          title="Total Components"
          value={totalComponents.toLocaleString()}
          icon={Cpu}
          iconClassName="bg-primary/10 text-primary"
        />
        <SummaryCard
          title="Storage Units"
          value={storageUnits.length}
          icon={Archive}
          iconClassName="bg-amber-100 text-amber-600 dark:bg-amber-900/50 dark:text-amber-400"
        />
        <SummaryCard
          title="Total Bins"
          value={totalBins}
          icon={Grid3x3}
          iconClassName="bg-emerald-100 text-emerald-600 dark:bg-emerald-900/50 dark:text-emerald-400"
        />
        <SummaryCard
          title="Compartments"
          value={storageUnits.reduce((sum, su) => sum + su.compartmentCount, 0)}
          icon={Layers}
          iconClassName="bg-purple-100 text-purple-600 dark:bg-purple-900/50 dark:text-purple-400"
        />
        <SummaryCard
          title="Low Stock"
          value={lowStock}
          icon={AlertTriangle}
          iconClassName="bg-red-100 text-red-600 dark:bg-red-900/50 dark:text-red-400"
        />
      </div>

      {/* Quick Actions */}
      <div>
        <h2 className="mb-3 text-lg font-semibold">Quick Actions</h2>
        <QuickActions />
      </div>

      {/* Storage Units + Recent Activity */}
      <div className="grid gap-6 lg:grid-cols-[1fr_300px]">
        <div className="space-y-6">
          {/* Storage Unit Preview Cards */}
          <div>
            <h2 className="mb-3 text-lg font-semibold">Storage Units</h2>
            <div className="grid grid-cols-2 gap-4 md:grid-cols-3 lg:grid-cols-5">
              {storageUnits.map((su, i) => (
                <StorageUnitPreview
                  key={su.id}
                  id={su.id}
                  name={su.name}
                  rows={su.rows}
                  columns={su.columns}
                  bins={su.rows * su.columns}
                  components={su.compartmentCount}
                  colorScheme={colorSchemes[i % colorSchemes.length]}
                />
              ))}
            </div>
          </div>

          {/* Storage Unit Detail Cards */}
          <div>
            <h2 className="mb-3 text-lg font-semibold">Storage Units</h2>
            <div className="grid grid-cols-1 gap-4 md:grid-cols-2 xl:grid-cols-4">
              {storageUnits.map((su, i) => (
                <StorageUnitDetailCard
                  key={su.id}
                  id={su.id}
                  name={su.name}
                  rows={su.rows}
                  columns={su.columns}
                  bins={su.rows * su.columns}
                  components={su.compartmentCount}
                  colorScheme={colorSchemes[i % colorSchemes.length]}
                />
              ))}
            </div>
          </div>
        </div>

        {/* Recent Activity Sidebar */}
        <div>
          <RecentActivity />
        </div>
      </div>
    </div>
  );
}
