"use client";

import { User, Sliders, Tag, Download, DollarSign } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { useAuth } from "@/lib/auth";
import { ThemeToggle } from "@/components/theme-toggle";
import { useCategories } from "@/hooks/use-categories";
import type { CategoryResponse } from "@/types/api";
import { Badge } from "@/components/ui/badge";
import { useCurrency, CURRENCIES } from "@/lib/currency";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

function flattenCategories(cats: CategoryResponse[]): string[] {
  return cats.flatMap((c) => [c.name, ...flattenCategories(c.children)]);
}

export default function SettingsPage() {
  const { user } = useAuth();
  const { currency, setCurrencyCode } = useCurrency();
  const { data: categoriesData } = useCategories();
  const categoryNames = flattenCategories(categoriesData ?? []);

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold">Settings</h1>
        <p className="text-muted-foreground">System configuration</p>
      </div>

      <div className="grid gap-6 lg:grid-cols-2">
        {/* Profile */}
        <div className="rounded-xl border bg-card p-5 shadow-sm">
          <h3 className="mb-4 flex items-center gap-2 font-semibold">
            <User className="h-4 w-4" /> Profile
          </h3>
          <div className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div>
                <Label>First Name</Label>
                <Input defaultValue={user?.firstName} className="mt-1" />
              </div>
              <div>
                <Label>Last Name</Label>
                <Input defaultValue={user?.lastName} className="mt-1" />
              </div>
            </div>
            <div>
              <Label>Email</Label>
              <Input defaultValue={user?.email} className="mt-1" disabled />
            </div>
            <Button>Save Profile</Button>
          </div>
        </div>

        {/* Appearance */}
        <div className="rounded-xl border bg-card p-5 shadow-sm">
          <h3 className="mb-4 flex items-center gap-2 font-semibold">
            <Sliders className="h-4 w-4" /> Appearance
          </h3>
          <div className="space-y-4">
            <div className="flex items-center justify-between rounded-lg border bg-background p-3">
              <div>
                <p className="font-medium">Theme</p>
                <p className="text-sm text-muted-foreground">
                  Toggle between light and dark mode
                </p>
              </div>
              <ThemeToggle />
            </div>
          </div>
        </div>

        {/* Currency */}
        <div className="rounded-xl border bg-card p-5 shadow-sm">
          <h3 className="mb-4 flex items-center gap-2 font-semibold">
            <DollarSign className="h-4 w-4" /> Currency
          </h3>
          <div className="space-y-4">
            <div className="flex items-center justify-between rounded-lg border bg-background p-3">
              <div>
                <p className="font-medium">Price Currency</p>
                <p className="text-sm text-muted-foreground">
                  Used for unit prices, reports, and exports
                </p>
              </div>
              <Select value={currency.code} onValueChange={setCurrencyCode}>
                <SelectTrigger className="w-[200px]">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {CURRENCIES.map((c) => (
                    <SelectItem key={c.code} value={c.code}>
                      {c.symbol} {c.code} — {c.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <p className="text-xs text-muted-foreground">
              Currently displaying prices as: {currency.symbol}1,234.56 ({currency.code})
            </p>
          </div>
        </div>

        {/* Categories */}
        <div className="rounded-xl border bg-card p-5 shadow-sm">
          <h3 className="mb-4 flex items-center gap-2 font-semibold">
            <Tag className="h-4 w-4" /> Component Categories
          </h3>
          <div className="flex flex-wrap gap-2">
            {categoryNames.map((cat) => (
              <Badge key={cat} variant="secondary" className="text-sm">
                {cat}
              </Badge>
            ))}
          </div>
          <Button variant="outline" className="mt-4" size="sm">
            Add Category
          </Button>
        </div>

        {/* Backup */}
        <div className="rounded-xl border bg-card p-5 shadow-sm">
          <h3 className="mb-4 flex items-center gap-2 font-semibold">
            <Download className="h-4 w-4" /> Backup / Export
          </h3>
          <div className="space-y-3">
            <p className="text-sm text-muted-foreground">
              Export your inventory data for backup or migration purposes.
            </p>
            <div className="flex gap-3">
              <Button variant="outline">Export as CSV</Button>
              <Button variant="outline">Export as JSON</Button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
