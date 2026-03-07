"use client";

import {
  LayoutDashboard,
  Cpu,
  Search,
  Archive,
  History,
  BarChart3,
  Settings,
  LogOut,
  Package,
} from "lucide-react";
import { useAuth } from "@/lib/auth";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { cn } from "@/lib/utils";
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
} from "@/components/ui/sidebar";

const navItems = [
  { label: "Dashboard", href: "/", icon: LayoutDashboard },
  { label: "Components", href: "/components", icon: Cpu },
  { label: "Find Component", href: "/find", icon: Search },
  { label: "Storage Units", href: "/storage-units", icon: Archive },
  { label: "Movement History", href: "/history", icon: History },
  { label: "Reports", href: "/reports", icon: BarChart3 },
  { label: "Settings", href: "/settings", icon: Settings },
];

export function AppSidebar() {
  const { user, logout } = useAuth();
  const pathname = usePathname();

  return (
    <Sidebar>
      <SidebarHeader className="p-5 pb-6">
        <Link href="/" className="flex items-center gap-2.5">
          <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-sidebar-primary">
            <Package className="h-4.5 w-4.5 text-sidebar-primary-foreground" />
          </div>
          <span className="text-lg font-bold text-white">Bintainer</span>
        </Link>
      </SidebarHeader>
      <SidebarContent className="px-3">
        <SidebarMenu>
          {navItems.map((item) => {
            const isActive =
              pathname === item.href ||
              (item.href !== "/" && pathname.startsWith(item.href));
            return (
              <SidebarMenuItem key={item.href}>
                <SidebarMenuButton
                  asChild
                  isActive={isActive}
                  className={cn(
                    "h-10 gap-3 rounded-lg px-3 font-medium transition-colors",
                    isActive
                      ? "bg-sidebar-primary text-sidebar-primary-foreground hover:bg-sidebar-primary/90"
                      : "text-sidebar-foreground hover:bg-sidebar-accent hover:text-sidebar-accent-foreground"
                  )}
                >
                  <Link href={item.href}>
                    <item.icon className="h-4.5 w-4.5" />
                    <span>{item.label}</span>
                  </Link>
                </SidebarMenuButton>
              </SidebarMenuItem>
            );
          })}
        </SidebarMenu>
      </SidebarContent>
      <SidebarFooter className="p-4">
        {user && (
          <div className="flex items-center justify-between rounded-lg bg-sidebar-accent/50 px-3 py-2.5">
            <div className="min-w-0">
              <p className="truncate text-sm font-medium text-white">
                {user.firstName} {user.lastName}
              </p>
              <p className="truncate text-xs text-sidebar-foreground">
                {user.email}
              </p>
            </div>
            <button
              onClick={logout}
              className="ml-2 shrink-0 rounded-md p-1.5 text-sidebar-foreground transition-colors hover:bg-sidebar-accent hover:text-white"
              title="Logout"
            >
              <LogOut className="h-4 w-4" />
            </button>
          </div>
        )}
      </SidebarFooter>
    </Sidebar>
  );
}
