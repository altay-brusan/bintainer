"use client";

import { useState } from "react";
import {
  AlertTriangle,
  TrendingUp,
  Archive,
  Package,
  DollarSign,
  FileSpreadsheet,
  FileText,
  Download,
  Calendar,
  BarChart3,
  PieChart as PieChartIcon,
  History,
} from "lucide-react";
import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { useCurrency } from "@/lib/currency";
import {
  useSummary,
  useTopComponents,
  useLowStock,
  useStorageUtilization,
  useMovementTimeline,
  useSupplierDistribution,
  useCategoryDistribution,
} from "@/hooks/use-reports";
import { useBomHistory } from "@/hooks/use-bom";
import { toast } from "sonner";
import {
  PieChart,
  Pie,
  Cell,
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  Legend,
  AreaChart,
  Area,
} from "recharts";

const CHART_COLORS = [
  "#3B82F6", "#10B981", "#F59E0B", "#8B5CF6", "#EF4444",
  "#06B6D4", "#F97316", "#EC4899", "#6366F1", "#14B8A6",
];

type TabKey = "overview" | "bom" | "inventory";

export default function ReportsPage() {
  const [activeTab, setActiveTab] = useState<TabKey>("overview");
  const [valueChartLimit, setValueChartLimit] = useState("10");
  const { format: fmt, currency } = useCurrency();

  // Fetch report data
  const { data: summary } = useSummary();
  const { data: topComponents } = useTopComponents("value", 10);
  const { data: lowStockItems } = useLowStock();
  const { data: storageUtil } = useStorageUtilization();
  const { data: timeline } = useMovementTimeline(7);
  const { data: supplierDist } = useSupplierDistribution();
  const { data: categoryDist } = useCategoryDistribution();
  const { data: bomHistory } = useBomHistory({ page: 1, pageSize: 20 });

  // Derived chart data
  const categoryChartData = (categoryDist ?? []).map((item) => ({
    name: item.categoryName,
    count: item.componentCount,
    value: item.totalValue,
  }));

  const valueByCategoryData = (categoryDist ?? [])
    .map((item) => ({
      name: item.categoryName,
      value: parseFloat(item.totalValue.toFixed(2)),
    }))
    .sort((a, b) => b.value - a.value);

  const timelineChartData = (timeline ?? []).map((item) => ({
    date: new Date(item.date).toLocaleDateString("en-US", {
      weekday: "short",
      month: "short",
      day: "numeric",
    }),
    count: item.count,
  }));

  const supplierChartData = (supplierDist ?? []).map((item) => ({
    name: item.supplierName,
    value: item.componentCount,
  }));

  const bomItems = bomHistory?.items ?? [];

  const handleExportExcel = () => {
    toast("Export feature coming soon");
  };

  const handleExportPdf = () => {
    toast("Export feature coming soon");
  };

  const tabs: { key: TabKey; label: string; icon: React.ReactNode }[] = [
    { key: "overview", label: "Overview", icon: <BarChart3 className="h-4 w-4" /> },
    { key: "bom", label: "BOM History", icon: <History className="h-4 w-4" /> },
    { key: "inventory", label: "Inventory Analysis", icon: <PieChartIcon className="h-4 w-4" /> },
  ];

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold">Reports</h1>
          <p className="text-muted-foreground">
            Inventory analytics, insights, and export
          </p>
        </div>
        <div className="flex gap-2">
          <Button variant="outline" className="gap-2" onClick={handleExportExcel}>
            <FileSpreadsheet className="h-4 w-4" />
            Export Excel
          </Button>
          <Button variant="outline" className="gap-2" onClick={handleExportPdf}>
            <FileText className="h-4 w-4" />
            Export PDF
          </Button>
        </div>
      </div>

      {/* Tabs */}
      <div className="flex gap-1 rounded-lg border p-1 bg-muted/30">
        {tabs.map((tab) => (
          <Button
            key={tab.key}
            variant={activeTab === tab.key ? "default" : "ghost"}
            size="sm"
            className="gap-2"
            onClick={() => setActiveTab(tab.key)}
          >
            {tab.icon}
            {tab.label}
          </Button>
        ))}
      </div>

      {/* Overview Tab */}
      {activeTab === "overview" && (
        <div className="space-y-6">
          {/* KPI Cards */}
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-5">
            <ReportCard
              title="Total Components"
              value={(summary?.totalQuantity ?? 0).toLocaleString()}
              subtitle={`${summary?.totalComponents ?? 0} unique items`}
              icon={Package}
              iconClassName="bg-primary/10 text-primary"
            />
            <ReportCard
              title="Inventory Value"
              value={fmt(summary?.totalValue ?? 0)}
              subtitle="Total stock value"
              icon={DollarSign}
              iconClassName="bg-emerald-100 text-emerald-600 dark:bg-emerald-900/50 dark:text-emerald-400"
            />
            <ReportCard
              title="Storage Units"
              value={(summary?.totalStorageUnits ?? 0).toString()}
              subtitle={`${summary?.occupiedCompartments ?? 0} occupied`}
              icon={Archive}
              iconClassName="bg-blue-100 text-blue-600 dark:bg-blue-900/50 dark:text-blue-400"
            />
            <ReportCard
              title="Low Stock Items"
              value={(lowStockItems?.length ?? 0).toString()}
              subtitle="Need restocking"
              icon={AlertTriangle}
              iconClassName="bg-red-100 text-red-600 dark:bg-red-900/50 dark:text-red-400"
            />
            <ReportCard
              title="Recent Activity"
              value={(summary?.recentMovements ?? 0).toString()}
              subtitle="Last 7 days"
              icon={TrendingUp}
              iconClassName="bg-amber-100 text-amber-600 dark:bg-amber-900/50 dark:text-amber-400"
            />
          </div>

          {/* Charts Row 1 */}
          <div className="grid gap-6 lg:grid-cols-2">
            {/* Category Distribution Pie */}
            <div className="rounded-xl border bg-card p-5 shadow-sm">
              <h3 className="mb-4 flex items-center gap-2 font-semibold">
                <PieChartIcon className="h-4 w-4 text-primary" />
                Component Distribution by Category
              </h3>
              <div className="h-[280px]">
                <ResponsiveContainer width="100%" height="100%">
                  <PieChart>
                    <Pie
                      data={categoryChartData}
                      cx="50%"
                      cy="50%"
                      outerRadius={100}
                      innerRadius={50}
                      dataKey="count"
                      nameKey="name"
                      label={({ name, percent }) =>
                        `${name} ${((percent ?? 0) * 100).toFixed(0)}%`
                      }
                      labelLine={true}
                    >
                      {categoryChartData.map((_, i) => (
                        <Cell key={i} fill={CHART_COLORS[i % CHART_COLORS.length]} />
                      ))}
                    </Pie>
                    <Tooltip
                      formatter={(value) => [Number(value).toLocaleString(), "Qty"]}
                    />
                  </PieChart>
                </ResponsiveContainer>
              </div>
            </div>

            {/* Value by Category Bar */}
            <div className="rounded-xl border bg-card p-5 shadow-sm">
              <h3 className="mb-4 flex items-center gap-2 font-semibold">
                <DollarSign className="h-4 w-4 text-emerald-600" />
                Inventory Value by Category
              </h3>
              <div className="h-[280px]">
                <ResponsiveContainer width="100%" height="100%">
                  <BarChart data={valueByCategoryData}>
                    <CartesianGrid strokeDasharray="3 3" className="stroke-muted" />
                    <XAxis
                      dataKey="name"
                      tick={{ fontSize: 11 }}
                      className="fill-muted-foreground"
                    />
                    <YAxis
                      tick={{ fontSize: 11 }}
                      className="fill-muted-foreground"
                      tickFormatter={(v) => `${currency.symbol}${v}`}
                    />
                    <Tooltip
                      formatter={(value) => [fmt(Number(value)), "Value"]}
                    />
                    <Bar dataKey="value" fill="#10B981" radius={[4, 4, 0, 0]} />
                  </BarChart>
                </ResponsiveContainer>
              </div>
            </div>
          </div>

          {/* Charts Row 2 */}
          <div className="grid gap-6 lg:grid-cols-2">
            {/* Activity Timeline */}
            <div className="rounded-xl border bg-card p-5 shadow-sm">
              <h3 className="mb-4 flex items-center gap-2 font-semibold">
                <Calendar className="h-4 w-4 text-primary" />
                Activity Timeline (Last 7 Days)
              </h3>
              <div className="h-[280px]">
                <ResponsiveContainer width="100%" height="100%">
                  <AreaChart data={timelineChartData}>
                    <CartesianGrid strokeDasharray="3 3" className="stroke-muted" />
                    <XAxis dataKey="date" tick={{ fontSize: 10 }} className="fill-muted-foreground" />
                    <YAxis tick={{ fontSize: 11 }} className="fill-muted-foreground" />
                    <Tooltip />
                    <Area type="monotone" dataKey="count" stroke="#3B82F6" fill="#3B82F6" fillOpacity={0.3} name="Movements" />
                  </AreaChart>
                </ResponsiveContainer>
              </div>
            </div>

            {/* Supplier Distribution */}
            <div className="rounded-xl border bg-card p-5 shadow-sm">
              <h3 className="mb-4 flex items-center gap-2 font-semibold">
                <PieChartIcon className="h-4 w-4 text-amber-500" />
                Components by Supplier
              </h3>
              <div className="h-[280px]">
                <ResponsiveContainer width="100%" height="100%">
                  <PieChart>
                    <Pie
                      data={supplierChartData}
                      cx="50%"
                      cy="50%"
                      outerRadius={100}
                      innerRadius={50}
                      dataKey="value"
                      nameKey="name"
                      label={({ name, percent }) =>
                        `${name} ${((percent ?? 0) * 100).toFixed(0)}%`
                      }
                      labelLine={true}
                    >
                      {supplierChartData.map((_, i) => (
                        <Cell key={i} fill={CHART_COLORS[(i + 3) % CHART_COLORS.length]} />
                      ))}
                    </Pie>
                    <Tooltip
                      formatter={(value) => [Number(value).toLocaleString(), "Qty"]}
                    />
                  </PieChart>
                </ResponsiveContainer>
              </div>
            </div>
          </div>

          {/* Low Stock + Most Used */}
          <div className="grid gap-6 lg:grid-cols-2">
            <div className="rounded-xl border bg-card p-5 shadow-sm">
              <h3 className="mb-4 flex items-center gap-2 font-semibold">
                <AlertTriangle className="h-4 w-4 text-destructive" />
                Low Stock Components
              </h3>
              {(lowStockItems ?? []).length === 0 ? (
                <p className="text-sm text-muted-foreground">All stock levels are healthy.</p>
              ) : (
                <div className="space-y-3">
                  {(lowStockItems ?? []).map((comp) => (
                    <div key={comp.id} className="flex items-center justify-between rounded-lg border bg-background p-3">
                      <div>
                        <p className="font-medium">{comp.partNumber}</p>
                        <p className="text-xs text-muted-foreground">
                          {comp.description}
                        </p>
                      </div>
                      <div className="text-right">
                        <p className="font-semibold text-destructive">{comp.totalQuantity}</p>
                        <p className="text-xs text-muted-foreground">
                          min: {comp.lowStockThreshold}
                        </p>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>

            <div className="rounded-xl border bg-card p-5 shadow-sm">
              <h3 className="mb-4 flex items-center gap-2 font-semibold">
                <TrendingUp className="h-4 w-4 text-primary" />
                Top Components by Value
              </h3>
              {(topComponents ?? []).length === 0 ? (
                <p className="text-sm text-muted-foreground">No component data yet.</p>
              ) : (
                <div className="space-y-3">
                  {(topComponents ?? []).map((comp, i) => (
                    <div key={comp.id} className="flex items-center gap-3">
                      <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-primary/10 text-sm font-bold text-primary">
                        {i + 1}
                      </div>
                      <div className="flex-1">
                        <p className="font-medium">{comp.partNumber}</p>
                        <p className="text-xs text-muted-foreground">{comp.description}</p>
                        <div className="mt-1 h-2 rounded-full bg-muted">
                          <div
                            className="h-2 rounded-full bg-primary"
                            style={{
                              width: `${(comp.totalValue / (topComponents?.[0]?.totalValue || 1)) * 100}%`,
                            }}
                          />
                        </div>
                      </div>
                      <span className="text-sm font-medium text-muted-foreground">
                        {fmt(comp.totalValue)}
                      </span>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>

          {/* Storage Utilization */}
          <div className="rounded-xl border bg-card p-5 shadow-sm">
            <h3 className="mb-4 flex items-center gap-2 font-semibold">
              <Archive className="h-4 w-4 text-emerald-600 dark:text-emerald-400" />
              Storage Utilization
            </h3>
            <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-5">
              {(storageUtil ?? []).map((su) => {
                const utilization = su.totalCompartments > 0 ? Math.round((su.occupiedCompartments / su.totalCompartments) * 100) : 0;
                return (
                  <div key={su.storageUnitId} className="rounded-lg border bg-background p-3 text-center">
                    <p className="font-medium">{su.storageUnitName}</p>
                    <p className="mt-2 text-2xl font-bold text-primary">{utilization}%</p>
                    <p className="text-xs text-muted-foreground">
                      {su.occupiedCompartments}/{su.totalCompartments} compartments used
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
      )}

      {/* BOM History Tab */}
      {activeTab === "bom" && (
        <div className="space-y-6">
          <div className="rounded-xl border bg-card shadow-sm">
            <div className="p-5 border-b">
              <h3 className="font-semibold flex items-center gap-2">
                <History className="h-4 w-4" />
                BOM Import History
              </h3>
              <p className="text-sm text-muted-foreground mt-1">
                Track all BOM file imports and their results
              </p>
            </div>
            <div className="divide-y">
              {bomItems.length === 0 ? (
                <p className="p-4 text-sm text-muted-foreground">No BOM imports yet.</p>
              ) : (
                bomItems.map((bom) => (
                  <div key={bom.id} className="flex items-center justify-between p-4 hover:bg-muted/30 transition-colors">
                    <div className="flex items-center gap-4">
                      <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-primary/10">
                        <FileSpreadsheet className="h-5 w-5 text-primary" />
                      </div>
                      <div>
                        <p className="font-medium">{bom.fileName}</p>
                        <div className="flex items-center gap-3 mt-0.5">
                          <span className="text-xs text-muted-foreground">
                            {new Date(bom.date).toLocaleDateString("en-US", {
                              year: "numeric",
                              month: "short",
                              day: "numeric",
                              hour: "2-digit",
                              minute: "2-digit",
                            })}
                          </span>
                        </div>
                      </div>
                    </div>
                    <div className="flex items-center gap-4">
                      <div className="text-right">
                        <div className="flex gap-2">
                          <Badge variant="secondary" className="text-xs">{bom.totalLines} lines</Badge>
                          <Badge variant="secondary" className="text-xs text-emerald-600">{bom.matchedCount} matched</Badge>
                          {bom.newCount > 0 && (
                            <Badge variant="secondary" className="text-xs text-amber-600">{bom.newCount} new</Badge>
                          )}
                        </div>
                        <p className="text-xs text-muted-foreground mt-1">
                          Value: {fmt(bom.totalValue)}
                        </p>
                      </div>
                      <Button variant="ghost" size="icon" className="h-8 w-8">
                        <Download className="h-4 w-4" />
                      </Button>
                    </div>
                  </div>
                ))
              )}
            </div>
          </div>

          {/* BOM Stats */}
          <div className="grid gap-6 lg:grid-cols-3">
            <ReportCard
              title="Total BOM Imports"
              value={bomItems.length.toString()}
              subtitle="All time"
              icon={FileSpreadsheet}
              iconClassName="bg-primary/10 text-primary"
            />
            <ReportCard
              title="Total Lines Processed"
              value={bomItems.reduce((s, b) => s + b.totalLines, 0).toString()}
              subtitle={`${bomItems.reduce((s, b) => s + b.matchedCount, 0)} matched`}
              icon={Package}
              iconClassName="bg-emerald-100 text-emerald-600 dark:bg-emerald-900/50 dark:text-emerald-400"
            />
            <ReportCard
              title="Total BOM Value"
              value={fmt(bomItems.reduce((s, b) => s + b.totalValue, 0))}
              subtitle="Across all imports"
              icon={DollarSign}
              iconClassName="bg-amber-100 text-amber-600 dark:bg-amber-900/50 dark:text-amber-400"
            />
          </div>
        </div>
      )}

      {/* Inventory Analysis Tab */}
      {activeTab === "inventory" && (
        <div className="space-y-6">
          {/* Top components by value */}
          {(() => {
            const allValueData = (topComponents ?? []).map((c) => ({
              name: c.partNumber,
              value: parseFloat(c.totalValue.toFixed(2)),
              quantity: c.totalQuantity,
            }));
            const valueData = valueChartLimit === "all" ? allValueData : allValueData.slice(0, Number(valueChartLimit));
            const chartHeight = Math.max(320, valueData.length * 32);
            return (
              <div className="rounded-xl border bg-card p-5 shadow-sm">
                <div className="flex items-center justify-between mb-4">
                  <h3 className="flex items-center gap-2 font-semibold">
                    <DollarSign className="h-4 w-4 text-emerald-600" />
                    Components by Total Value
                  </h3>
                  <ChartLimitSelect value={valueChartLimit} onChange={setValueChartLimit} total={allValueData.length} />
                </div>
                <div style={{ height: Math.min(chartHeight, 600) }} className="overflow-y-auto">
                  <div style={{ height: chartHeight }}>
                    <ResponsiveContainer width="100%" height="100%">
                      <BarChart data={valueData} layout="vertical" margin={{ left: 120 }}>
                        <CartesianGrid strokeDasharray="3 3" className="stroke-muted" />
                        <XAxis type="number" tick={{ fontSize: 11 }} tickFormatter={(v) => `${currency.symbol}${v}`} />
                        <YAxis type="category" dataKey="name" tick={{ fontSize: 11 }} width={110} />
                        <Tooltip
                          formatter={(value, _name, props) => {
                            const p = props?.payload as { quantity?: number } | undefined;
                            return [
                              `${fmt(Number(value))} (qty: ${p?.quantity ?? 0})`,
                              "Total Value",
                            ];
                          }}
                        />
                        <Bar dataKey="value" fill="#3B82F6" radius={[0, 4, 4, 0]} />
                      </BarChart>
                    </ResponsiveContainer>
                  </div>
                </div>
              </div>
            );
          })()}

          {/* Low stock chart */}
          {(() => {
            const stockData = (lowStockItems ?? []).map((c) => ({
              name: c.partNumber,
              stock: c.totalQuantity,
              threshold: c.lowStockThreshold,
            }));
            const chartHeight = Math.max(280, stockData.length > 10 ? stockData.length * 28 : 280);
            return stockData.length > 0 ? (
              <div className="rounded-xl border bg-card p-5 shadow-sm">
                <div className="mb-4">
                  <h3 className="flex items-center gap-2 font-semibold">
                    <AlertTriangle className="h-4 w-4 text-destructive" />
                    Low Stock Levels vs. Thresholds
                  </h3>
                  <p className="text-xs text-muted-foreground mt-1">
                    Components below their minimum stock threshold
                  </p>
                </div>
                <div style={{ height: Math.min(chartHeight, 500) }} className="overflow-y-auto">
                  <div style={{ height: chartHeight }}>
                    <ResponsiveContainer width="100%" height="100%">
                      <BarChart data={stockData} layout="vertical" margin={{ left: 120 }}>
                        <CartesianGrid strokeDasharray="3 3" className="stroke-muted" />
                        <XAxis type="number" tick={{ fontSize: 11 }} />
                        <YAxis type="category" dataKey="name" tick={{ fontSize: 11 }} width={110} />
                        <Tooltip />
                        <Legend />
                        <Bar dataKey="stock" fill="#3B82F6" name="Current Stock" radius={[0, 4, 4, 0]} />
                        <Bar dataKey="threshold" fill="#EF4444" name="Low Threshold" radius={[0, 4, 4, 0]} />
                      </BarChart>
                    </ResponsiveContainer>
                  </div>
                </div>
              </div>
            ) : (
              <div className="rounded-xl border bg-card p-5 shadow-sm">
                <h3 className="flex items-center gap-2 font-semibold">
                  <AlertTriangle className="h-4 w-4 text-destructive" />
                  Low Stock Levels vs. Thresholds
                </h3>
                <p className="mt-2 text-sm text-muted-foreground">All stock levels are healthy.</p>
              </div>
            );
          })()}

          {/* Summary cards */}
          <div className="grid gap-6 lg:grid-cols-3">
            <ReportCard
              title="Total Quantity"
              value={(summary?.totalQuantity ?? 0).toLocaleString()}
              subtitle={`${summary?.totalComponents ?? 0} unique components`}
              icon={Package}
              iconClassName="bg-primary/10 text-primary"
            />
            <ReportCard
              title="Total Value"
              value={fmt(summary?.totalValue ?? 0)}
              subtitle="Across all components"
              icon={DollarSign}
              iconClassName="bg-emerald-100 text-emerald-600 dark:bg-emerald-900/50 dark:text-emerald-400"
            />
            <ReportCard
              title="Categories"
              value={(summary?.totalCategories ?? 0).toString()}
              subtitle={`${summary?.totalStorageUnits ?? 0} storage units`}
              icon={Archive}
              iconClassName="bg-blue-100 text-blue-600 dark:bg-blue-900/50 dark:text-blue-400"
            />
          </div>
        </div>
      )}
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

function ChartLimitSelect({
  value,
  onChange,
  total,
}: {
  value: string;
  onChange: (v: string) => void;
  total: number;
}) {
  return (
    <Select value={value} onValueChange={onChange}>
      <SelectTrigger className="h-7 w-[120px] text-xs">
        <SelectValue />
      </SelectTrigger>
      <SelectContent>
        <SelectItem value="10">Top 10</SelectItem>
        <SelectItem value="25">Top 25</SelectItem>
        <SelectItem value="50">Top 50</SelectItem>
        <SelectItem value="all">All ({total})</SelectItem>
      </SelectContent>
    </Select>
  );
}
