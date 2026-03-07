"use client";

import { useState, useMemo } from "react";
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
  demoComponents,
  demoStorageUnits,
  demoMovements,
  demoBomHistory,
} from "@/lib/demo-data";
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
  LineChart,
  Line,
  AreaChart,
  Area,
} from "recharts";

// --- Data computations ---

const lowStockComponents = demoComponents.filter(
  (c) => c.quantity <= c.lowStockThreshold
);

const totalComponents = demoComponents.reduce((sum, c) => sum + c.quantity, 0);
const totalInventoryValue = demoComponents.reduce(
  (sum, c) => sum + c.quantity * (c.unitPrice ?? 0),
  0
);
const uniqueItems = demoComponents.length;

// Category distribution
const categoryDistribution = Object.entries(
  demoComponents.reduce<Record<string, { count: number; value: number }>>(
    (acc, c) => {
      if (!acc[c.category]) acc[c.category] = { count: 0, value: 0 };
      acc[c.category].count += c.quantity;
      acc[c.category].value += c.quantity * (c.unitPrice ?? 0);
      return acc;
    },
    {}
  )
).map(([name, data]) => ({ name, ...data }));

// Price distribution by category
const priceByCategory = categoryDistribution
  .map((c) => ({ name: c.name, value: parseFloat(c.value.toFixed(2)) }))
  .sort((a, b) => b.value - a.value);

// Most used components
const mostUsed = demoMovements
  .filter((m) => m.action === "used")
  .reduce<Record<string, number>>((acc, m) => {
    acc[m.component] = (acc[m.component] || 0) + Math.abs(m.quantity);
    return acc;
  }, {});

const mostUsedSorted = Object.entries(mostUsed)
  .sort(([, a], [, b]) => b - a)
  .slice(0, 5);

// Movement timeline (last 7 days)
const movementTimeline = (() => {
  const days: { date: string; added: number; used: number; moved: number }[] = [];
  for (let i = 6; i >= 0; i--) {
    const d = new Date();
    d.setDate(d.getDate() - i);
    const dateStr = d.toISOString().split("T")[0];
    const dayLabel = d.toLocaleDateString("en-US", { weekday: "short", month: "short", day: "numeric" });
    const dayMovements = demoMovements.filter(
      (m) => m.date.startsWith(dateStr)
    );
    days.push({
      date: dayLabel,
      added: dayMovements.filter((m) => m.action === "added" || m.action === "restocked").reduce((s, m) => s + Math.abs(m.quantity), 0),
      used: dayMovements.filter((m) => m.action === "used").reduce((s, m) => s + Math.abs(m.quantity), 0),
      moved: dayMovements.filter((m) => m.action === "moved").length,
    });
  }
  return days;
})();

// Supplier distribution
const supplierDistribution = Object.entries(
  demoComponents.reduce<Record<string, number>>((acc, c) => {
    const supplier = c.supplier ?? "Unknown";
    acc[supplier] = (acc[supplier] || 0) + c.quantity;
    return acc;
  }, {})
).map(([name, value]) => ({ name, value }));

const CHART_COLORS = [
  "#3B82F6", "#10B981", "#F59E0B", "#8B5CF6", "#EF4444",
  "#06B6D4", "#F97316", "#EC4899", "#6366F1", "#14B8A6",
];

type TabKey = "overview" | "bom" | "inventory";

export default function ReportsPage() {
  const [activeTab, setActiveTab] = useState<TabKey>("overview");
  const [valueChartLimit, setValueChartLimit] = useState("10");
  const [priceChartLimit, setPriceChartLimit] = useState("10");
  const [stockChartLimit, setStockChartLimit] = useState("10");
  const { format: fmt, currency } = useCurrency();

  const handleExportExcel = () => {
    import("xlsx").then((XLSX) => {
      const wsData = demoComponents.map((c) => ({
        Name: c.name,
        "Part Number": c.partNumber ?? "",
        Category: c.category,
        "Storage Unit": c.storageUnit,
        Bin: c.bin,
        Compartment: c.compartment,
        Quantity: c.quantity,
        "Unit Price": c.unitPrice ?? 0,
        "Total Value": c.quantity * (c.unitPrice ?? 0),
        Supplier: c.supplier ?? "",
        Package: c.package ?? "",
        "Low Stock Threshold": c.lowStockThreshold,
        Tags: (c.tags ?? []).join(", "),
      }));
      const wb = XLSX.utils.book_new();
      const ws = XLSX.utils.json_to_sheet(wsData);

      // Set column widths
      ws["!cols"] = [
        { wch: 20 }, { wch: 22 }, { wch: 16 }, { wch: 16 },
        { wch: 10 }, { wch: 12 }, { wch: 10 }, { wch: 12 },
        { wch: 12 }, { wch: 12 }, { wch: 10 }, { wch: 18 }, { wch: 20 },
      ];

      XLSX.utils.book_append_sheet(wb, ws, "Inventory");

      // BOM History sheet
      const bomWsData = demoBomHistory.map((b) => ({
        Date: new Date(b.date).toLocaleDateString(),
        "File Name": b.fileName,
        "Total Lines": b.totalLines,
        "Matched Parts": b.matchedParts,
        "New Parts": b.newParts,
        "Total Value": b.totalValue,
        User: b.user,
      }));
      const bomWs = XLSX.utils.json_to_sheet(bomWsData);
      XLSX.utils.book_append_sheet(wb, bomWs, "BOM History");

      XLSX.writeFile(wb, `bintainer-inventory-report-${new Date().toISOString().split("T")[0]}.xlsx`);
    });
  };

  const handleExportPdf = () => {
    import("jspdf").then(({ jsPDF }) => {
      import("jspdf-autotable").then((autoTableModule) => {
        const doc = new jsPDF({ orientation: "landscape" });
        const autoTable = autoTableModule.default;

        // Title
        doc.setFontSize(18);
        doc.text("Bintainer Inventory Report", 14, 20);
        doc.setFontSize(10);
        doc.text(`Generated: ${new Date().toLocaleString()}`, 14, 28);

        // Summary
        doc.setFontSize(12);
        doc.text("Summary", 14, 38);
        doc.setFontSize(10);
        doc.text(`Total Components: ${totalComponents.toLocaleString()} (${uniqueItems} unique)`, 14, 45);
        doc.text(`Total Inventory Value: ${fmt(totalInventoryValue)}`, 14, 51);
        doc.text(`Low Stock Items: ${lowStockComponents.length}`, 14, 57);
        doc.text(`Storage Units: ${demoStorageUnits.length}`, 14, 63);

        // Components table
        const tableData = demoComponents.map((c) => [
          c.name,
          c.partNumber ?? "",
          c.category,
          c.storageUnit,
          c.bin,
          c.quantity.toString(),
          c.unitPrice != null ? fmt(c.unitPrice) : "",
          fmt(c.quantity * (c.unitPrice ?? 0)),
        ]);

        autoTable(doc, {
          head: [["Component", "Part Number", "Category", "Storage Unit", "Bin", "Qty", "Unit Price", "Total"]],
          body: tableData,
          startY: 70,
          styles: { fontSize: 8 },
          headStyles: { fillColor: [59, 130, 246] },
        });

        doc.save(`bintainer-inventory-report-${new Date().toISOString().split("T")[0]}.pdf`);
      });
    });
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
              value={totalComponents.toLocaleString()}
              subtitle={`${uniqueItems} unique items`}
              icon={Package}
              iconClassName="bg-primary/10 text-primary"
            />
            <ReportCard
              title="Inventory Value"
              value={fmt(totalInventoryValue)}
              subtitle="Total stock value"
              icon={DollarSign}
              iconClassName="bg-emerald-100 text-emerald-600 dark:bg-emerald-900/50 dark:text-emerald-400"
            />
            <ReportCard
              title="Storage Units"
              value={demoStorageUnits.length.toString()}
              subtitle={`${demoStorageUnits.reduce((s, u) => s + u.rows * u.columns, 0)} total bins`}
              icon={Archive}
              iconClassName="bg-blue-100 text-blue-600 dark:bg-blue-900/50 dark:text-blue-400"
            />
            <ReportCard
              title="Low Stock Items"
              value={lowStockComponents.length.toString()}
              subtitle="Need restocking"
              icon={AlertTriangle}
              iconClassName="bg-red-100 text-red-600 dark:bg-red-900/50 dark:text-red-400"
            />
            <ReportCard
              title="Recent Activity"
              value={demoMovements.length.toString()}
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
                      data={categoryDistribution}
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
                      {categoryDistribution.map((_, i) => (
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
                  <BarChart data={priceByCategory}>
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
                  <AreaChart data={movementTimeline}>
                    <CartesianGrid strokeDasharray="3 3" className="stroke-muted" />
                    <XAxis dataKey="date" tick={{ fontSize: 10 }} className="fill-muted-foreground" />
                    <YAxis tick={{ fontSize: 11 }} className="fill-muted-foreground" />
                    <Tooltip />
                    <Legend />
                    <Area type="monotone" dataKey="added" stackId="1" stroke="#10B981" fill="#10B981" fillOpacity={0.3} name="Added/Restocked" />
                    <Area type="monotone" dataKey="used" stackId="2" stroke="#EF4444" fill="#EF4444" fillOpacity={0.3} name="Used" />
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
                      data={supplierDistribution}
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
                      {supplierDistribution.map((_, i) => (
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
                          {comp.unitPrice != null && (
                            <> &middot; {fmt(comp.unitPrice)}/ea</>
                          )}
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
          </div>

          {/* Storage Utilization */}
          <div className="rounded-xl border bg-card p-5 shadow-sm">
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
              {demoBomHistory.map((bom) => (
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
                        <span className="text-xs text-muted-foreground">by {bom.user}</span>
                      </div>
                    </div>
                  </div>
                  <div className="flex items-center gap-4">
                    <div className="text-right">
                      <div className="flex gap-2">
                        <Badge variant="secondary" className="text-xs">{bom.totalLines} lines</Badge>
                        <Badge variant="secondary" className="text-xs text-emerald-600">{bom.matchedParts} matched</Badge>
                        {bom.newParts > 0 && (
                          <Badge variant="secondary" className="text-xs text-amber-600">{bom.newParts} new</Badge>
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
              ))}
            </div>
          </div>

          {/* BOM Stats */}
          <div className="grid gap-6 lg:grid-cols-3">
            <ReportCard
              title="Total BOM Imports"
              value={demoBomHistory.length.toString()}
              subtitle="All time"
              icon={FileSpreadsheet}
              iconClassName="bg-primary/10 text-primary"
            />
            <ReportCard
              title="Total Lines Processed"
              value={demoBomHistory.reduce((s, b) => s + b.totalLines, 0).toString()}
              subtitle={`${demoBomHistory.reduce((s, b) => s + b.matchedParts, 0)} matched`}
              icon={Package}
              iconClassName="bg-emerald-100 text-emerald-600 dark:bg-emerald-900/50 dark:text-emerald-400"
            />
            <ReportCard
              title="Total BOM Value"
              value={fmt(demoBomHistory.reduce((s, b) => s + b.totalValue, 0))}
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
            const allValueData = demoComponents
              .map((c) => ({
                name: c.name,
                value: parseFloat((c.quantity * (c.unitPrice ?? 0)).toFixed(2)),
                quantity: c.quantity,
                unitPrice: c.unitPrice ?? 0,
              }))
              .sort((a, b) => b.value - a.value);
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
                            const p = props?.payload as { quantity?: number; unitPrice?: number } | undefined;
                            return [
                              `${fmt(Number(value))} (${p?.quantity ?? 0} x ${fmt(p?.unitPrice ?? 0)})`,
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

          {/* Price per unit comparison */}
          <div className="grid gap-6 lg:grid-cols-2">
            {(() => {
              const allPriceData = demoComponents
                .filter((c) => c.unitPrice != null && c.unitPrice > 0)
                .sort((a, b) => (b.unitPrice ?? 0) - (a.unitPrice ?? 0))
                .map((c) => ({ name: c.name, price: c.unitPrice }));
              const priceData = priceChartLimit === "all" ? allPriceData : allPriceData.slice(0, Number(priceChartLimit));
              const chartHeight = Math.max(280, priceData.length > 10 ? priceData.length * 28 : 280);
              return (
                <div className="rounded-xl border bg-card p-5 shadow-sm">
                  <div className="flex items-center justify-between mb-4">
                    <h3 className="flex items-center gap-2 font-semibold">
                      <BarChart3 className="h-4 w-4 text-primary" />
                      Most Expensive Components
                    </h3>
                    <ChartLimitSelect value={priceChartLimit} onChange={setPriceChartLimit} total={allPriceData.length} />
                  </div>
                  <div style={{ height: Math.min(chartHeight, 500) }} className="overflow-y-auto">
                    <div style={{ height: chartHeight }}>
                      <ResponsiveContainer width="100%" height="100%">
                        <BarChart data={priceData} layout="vertical" margin={{ left: 120 }}>
                          <CartesianGrid strokeDasharray="3 3" className="stroke-muted" />
                          <XAxis type="number" tick={{ fontSize: 11 }} tickFormatter={(v) => `${currency.symbol}${v}`} />
                          <YAxis type="category" dataKey="name" tick={{ fontSize: 11 }} width={110} />
                          <Tooltip formatter={(value) => [fmt(Number(value), 3), "Unit Price"]} />
                          <Bar dataKey="price" fill="#8B5CF6" radius={[0, 4, 4, 0]} />
                        </BarChart>
                      </ResponsiveContainer>
                    </div>
                  </div>
                </div>
              );
            })()}

            {(() => {
              const allStockData = demoComponents
                .map((c) => ({
                  name: c.name,
                  stock: c.quantity,
                  threshold: c.lowStockThreshold,
                  ratio: c.lowStockThreshold > 0 ? c.quantity / c.lowStockThreshold : Infinity,
                }))
                .sort((a, b) => a.ratio - b.ratio);
              const stockData = stockChartLimit === "all" ? allStockData : allStockData.slice(0, Number(stockChartLimit));
              const chartHeight = Math.max(280, stockData.length > 10 ? stockData.length * 28 : 280);
              return (
                <div className="rounded-xl border bg-card p-5 shadow-sm">
                  <div className="flex items-center justify-between mb-4">
                    <h3 className="flex items-center gap-2 font-semibold">
                      <Package className="h-4 w-4 text-primary" />
                      Stock Levels vs. Thresholds
                    </h3>
                    <ChartLimitSelect value={stockChartLimit} onChange={setStockChartLimit} total={allStockData.length} />
                  </div>
                  <p className="text-xs text-muted-foreground -mt-2 mb-3">
                    Sorted by most critical (closest to threshold)
                  </p>
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
              );
            })()}
          </div>

          {/* Full inventory table for export context */}
          <div className="rounded-xl border bg-card p-5 shadow-sm">
            <div className="flex items-center justify-between mb-4">
              <h3 className="font-semibold flex items-center gap-2">
                <Package className="h-4 w-4" />
                Complete Inventory Summary
              </h3>
              <div className="flex gap-2">
                <Button variant="outline" size="sm" className="gap-1.5" onClick={handleExportExcel}>
                  <FileSpreadsheet className="h-3.5 w-3.5" />
                  Excel
                </Button>
                <Button variant="outline" size="sm" className="gap-1.5" onClick={handleExportPdf}>
                  <FileText className="h-3.5 w-3.5" />
                  PDF
                </Button>
              </div>
            </div>
            <div className="rounded-lg border overflow-hidden">
              <div className="grid grid-cols-[1fr_120px_100px_80px_80px_90px] gap-2 px-4 py-2 text-xs font-medium text-muted-foreground bg-muted/50 border-b">
                <span>Component</span>
                <span>Part Number</span>
                <span>Category</span>
                <span>Qty</span>
                <span>Unit Price</span>
                <span>Total Value</span>
              </div>
              {demoComponents.map((c) => (
                <div key={c.id} className="grid grid-cols-[1fr_120px_100px_80px_80px_90px] gap-2 items-center px-4 py-2 text-sm border-b last:border-0">
                  <span className="font-medium truncate">{c.name}</span>
                  <span className="font-mono text-xs text-muted-foreground truncate">{c.partNumber ?? "—"}</span>
                  <span className="text-muted-foreground">{c.category}</span>
                  <span className={c.quantity <= c.lowStockThreshold ? "text-destructive font-semibold" : ""}>
                    {c.quantity}
                  </span>
                  <span className="font-mono text-xs">{c.unitPrice != null ? fmt(c.unitPrice) : "—"}</span>
                  <span className="font-mono text-xs font-medium">{fmt(c.quantity * (c.unitPrice ?? 0))}</span>
                </div>
              ))}
              <div className="grid grid-cols-[1fr_120px_100px_80px_80px_90px] gap-2 px-4 py-3 text-sm font-semibold bg-muted/30 border-t">
                <span>Total</span>
                <span />
                <span />
                <span>{totalComponents}</span>
                <span />
                <span className="font-mono">{fmt(totalInventoryValue)}</span>
              </div>
            </div>
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
