"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Plus } from "lucide-react";
import { toast } from "sonner";
import { useInventories, useCreateInventory } from "@/hooks/use-inventories";
import {
  createInventorySchema,
  type CreateInventoryInput,
} from "@/lib/validators";
import { InventoryCard } from "@/components/inventory-card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Skeleton } from "@/components/ui/skeleton";

export default function DashboardPage() {
  const { data: inventories, isLoading } = useInventories();
  const createInventory = useCreateInventory();
  const [open, setOpen] = useState(false);

  const form = useForm<CreateInventoryInput>({
    resolver: zodResolver(createInventorySchema),
    defaultValues: { name: "" },
  });

  const onSubmit = async (values: CreateInventoryInput) => {
    try {
      await createInventory.mutateAsync(values);
      toast.success("Inventory created");
      form.reset();
      setOpen(false);
    } catch {
      toast.error("Failed to create inventory");
    }
  };

  return (
    <div>
      <div className="mb-6 flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold">Inventories</h1>
          <p className="text-muted-foreground">
            Manage your storage inventories
          </p>
        </div>
        <Dialog open={open} onOpenChange={setOpen}>
          <DialogTrigger asChild>
            <Button>
              <Plus className="mr-2 h-4 w-4" />
              Create Inventory
            </Button>
          </DialogTrigger>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>Create Inventory</DialogTitle>
            </DialogHeader>
            <Form {...form}>
              <form
                onSubmit={form.handleSubmit(onSubmit)}
                className="space-y-4"
              >
                <FormField
                  control={form.control}
                  name="name"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Name</FormLabel>
                      <FormControl>
                        <Input placeholder="My Inventory" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                <Button
                  type="submit"
                  className="w-full"
                  disabled={createInventory.isPending}
                >
                  {createInventory.isPending ? "Creating..." : "Create"}
                </Button>
              </form>
            </Form>
          </DialogContent>
        </Dialog>
      </div>

      {isLoading ? (
        <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
          {Array.from({ length: 3 }).map((_, i) => (
            <Skeleton key={i} className="h-24 rounded-lg" />
          ))}
        </div>
      ) : inventories && inventories.length > 0 ? (
        <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
          {inventories.map((inventory) => (
            <InventoryCard key={inventory.id} inventory={inventory} />
          ))}
        </div>
      ) : (
        <div className="flex flex-col items-center justify-center rounded-lg border border-dashed p-12 text-center">
          <p className="text-lg font-medium">No inventories yet</p>
          <p className="text-sm text-muted-foreground">
            Create your first inventory to get started
          </p>
        </div>
      )}
    </div>
  );
}
