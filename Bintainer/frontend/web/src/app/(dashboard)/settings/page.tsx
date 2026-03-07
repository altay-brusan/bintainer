"use client";

import { User, Sliders, Tag, Download } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { useAuth } from "@/lib/auth";
import { ThemeToggle } from "@/components/theme-toggle";
import { categories } from "@/lib/demo-data";
import { Badge } from "@/components/ui/badge";

export default function SettingsPage() {
  const { user } = useAuth();

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

        {/* Categories */}
        <div className="rounded-xl border bg-card p-5 shadow-sm">
          <h3 className="mb-4 flex items-center gap-2 font-semibold">
            <Tag className="h-4 w-4" /> Component Categories
          </h3>
          <div className="flex flex-wrap gap-2">
            {categories.map((cat) => (
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
