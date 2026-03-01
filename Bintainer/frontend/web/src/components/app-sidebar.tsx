"use client";

import { Package, LogOut } from "lucide-react";
import { useAuth } from "@/lib/auth";
import Link from "next/link";
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarGroupContent,
  SidebarGroupLabel,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
} from "@/components/ui/sidebar";
import { Separator } from "@/components/ui/separator";

export function AppSidebar() {
  const { user, logout } = useAuth();

  return (
    <Sidebar>
      <SidebarHeader className="p-4">
        <Link href="/" className="flex items-center gap-2 font-semibold">
          <Package className="h-6 w-6" />
          <span className="text-lg">Bintainer</span>
        </Link>
      </SidebarHeader>
      <Separator />
      <SidebarContent>
        <SidebarGroup>
          <SidebarGroupLabel>Navigation</SidebarGroupLabel>
          <SidebarGroupContent>
            <SidebarMenu>
              <SidebarMenuItem>
                <SidebarMenuButton asChild>
                  <Link href="/">
                    <Package className="h-4 w-4" />
                    <span>Inventories</span>
                  </Link>
                </SidebarMenuButton>
              </SidebarMenuItem>
            </SidebarMenu>
          </SidebarGroupContent>
        </SidebarGroup>
      </SidebarContent>
      <Separator />
      <SidebarFooter className="p-4">
        {user && (
          <div className="flex items-center justify-between">
            <div className="text-sm">
              <p className="font-medium">
                {user.firstName} {user.lastName}
              </p>
              <p className="text-muted-foreground text-xs">{user.email}</p>
            </div>
            <button
              onClick={logout}
              className="rounded-md p-2 text-muted-foreground hover:bg-accent hover:text-accent-foreground"
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
