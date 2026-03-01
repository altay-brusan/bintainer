"use client";

import { useState } from "react";
import { useParams } from "next/navigation";
import Link from "next/link";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { ArrowLeft, Plus } from "lucide-react";
import { toast } from "sonner";
import { useStorageUnits, useCreateStorageUnit } from "@/hooks/use-storage-units";
import {
  createStorageUnitSchema,
  type CreateStorageUnitInput,
} from "@/lib/validators";
import { StorageUnitCard } from "@/components/storage-unit-card";
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

export default function InventoryDetailPage() {
  const { inventoryId } = useParams<{ inventoryId: string }>();
  const { data: storageUnits, isLoading } = useStorageUnits(inventoryId);
  const createStorageUnit = useCreateStorageUnit();
  const [open, setOpen] = useState(false);

  const form = useForm<CreateStorageUnitInput>({
    resolver: zodResolver(createStorageUnitSchema),
    defaultValues: {
      name: "",
      columns: 1,
      rows: 1,
      compartmentCount: 1,
      inventoryId,
    },
  });

  const onSubmit = async (values: CreateStorageUnitInput) => {
    try {
      await createStorageUnit.mutateAsync(values);
      toast.success("Storage unit created");
      form.reset({ name: "", columns: 1, rows: 1, compartmentCount: 1, inventoryId });
      setOpen(false);
    } catch {
      toast.error("Failed to create storage unit");
    }
  };

  return (
    <div>
      <div className="mb-6">
        <Link
          href="/"
          className="mb-4 inline-flex items-center text-sm text-muted-foreground hover:text-foreground"
        >
          <ArrowLeft className="mr-1 h-4 w-4" />
          Back to inventories
        </Link>
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-2xl font-bold">Storage Units</h1>
            <p className="text-muted-foreground">
              Manage storage units in this inventory
            </p>
          </div>
          <Dialog open={open} onOpenChange={setOpen}>
            <DialogTrigger asChild>
              <Button>
                <Plus className="mr-2 h-4 w-4" />
                Create Storage Unit
              </Button>
            </DialogTrigger>
            <DialogContent>
              <DialogHeader>
                <DialogTitle>Create Storage Unit</DialogTitle>
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
                          <Input placeholder="Shelf A" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <div className="grid grid-cols-3 gap-4">
                    <FormField
                      control={form.control}
                      name="columns"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>Columns</FormLabel>
                          <FormControl>
                            <Input
                              type="number"
                              min={1}
                              {...field}
                              onChange={(e) =>
                                field.onChange(parseInt(e.target.value) || 0)
                              }
                            />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                    <FormField
                      control={form.control}
                      name="rows"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>Rows</FormLabel>
                          <FormControl>
                            <Input
                              type="number"
                              min={1}
                              {...field}
                              onChange={(e) =>
                                field.onChange(parseInt(e.target.value) || 0)
                              }
                            />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                    <FormField
                      control={form.control}
                      name="compartmentCount"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>Compartments</FormLabel>
                          <FormControl>
                            <Input
                              type="number"
                              min={1}
                              {...field}
                              onChange={(e) =>
                                field.onChange(parseInt(e.target.value) || 0)
                              }
                            />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                  </div>
                  <Button
                    type="submit"
                    className="w-full"
                    disabled={createStorageUnit.isPending}
                  >
                    {createStorageUnit.isPending ? "Creating..." : "Create"}
                  </Button>
                </form>
              </Form>
            </DialogContent>
          </Dialog>
        </div>
      </div>

      {isLoading ? (
        <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
          {Array.from({ length: 3 }).map((_, i) => (
            <Skeleton key={i} className="h-24 rounded-lg" />
          ))}
        </div>
      ) : storageUnits && storageUnits.length > 0 ? (
        <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
          {storageUnits.map((unit) => (
            <StorageUnitCard key={unit.id} storageUnit={unit} />
          ))}
        </div>
      ) : (
        <div className="flex flex-col items-center justify-center rounded-lg border border-dashed p-12 text-center">
          <p className="text-lg font-medium">No storage units yet</p>
          <p className="text-sm text-muted-foreground">
            Create your first storage unit to get started
          </p>
        </div>
      )}
    </div>
  );
}
