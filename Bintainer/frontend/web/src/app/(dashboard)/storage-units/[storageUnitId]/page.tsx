"use client";

import { useState } from "react";
import { useParams, useRouter } from "next/navigation";
import Link from "next/link";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { ArrowLeft, Pencil, Trash2 } from "lucide-react";
import { toast } from "sonner";
import {
  useStorageUnit,
  useUpdateStorageUnit,
  useDeleteStorageUnit,
} from "@/hooks/use-storage-units";
import { updateStorageUnitSchema, type UpdateStorageUnitInput } from "@/lib/validators";
import { StorageUnitGrid } from "@/components/storage-unit-grid";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Badge } from "@/components/ui/badge";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Skeleton } from "@/components/ui/skeleton";

export default function StorageUnitDetailPage() {
  const { storageUnitId } = useParams<{ storageUnitId: string }>();
  const router = useRouter();
  const { data: storageUnit, isLoading } = useStorageUnit(storageUnitId);
  const updateStorageUnit = useUpdateStorageUnit();
  const deleteStorageUnit = useDeleteStorageUnit();

  const [editOpen, setEditOpen] = useState(false);
  const [deleteOpen, setDeleteOpen] = useState(false);

  const form = useForm<UpdateStorageUnitInput>({
    resolver: zodResolver(updateStorageUnitSchema),
    values: {
      name: storageUnit?.name ?? "",
    },
  });

  const onUpdate = async (values: UpdateStorageUnitInput) => {
    try {
      await updateStorageUnit.mutateAsync({ id: storageUnitId, ...values });
      toast.success("Storage unit updated");
      setEditOpen(false);
    } catch {
      toast.error("Failed to update storage unit");
    }
  };

  const onDelete = async () => {
    try {
      await deleteStorageUnit.mutateAsync(storageUnitId);
      toast.success("Storage unit deleted");
      if (storageUnit) {
        router.push(`/inventories/${storageUnit.inventoryId}`);
      } else {
        router.push("/");
      }
    } catch {
      toast.error("Failed to delete storage unit");
    }
  };

  if (isLoading) {
    return (
      <div className="space-y-6">
        <Skeleton className="h-8 w-48" />
        <Skeleton className="h-64 w-full" />
      </div>
    );
  }

  if (!storageUnit) {
    return (
      <div className="flex flex-col items-center justify-center p-12 text-center">
        <p className="text-lg font-medium">Storage unit not found</p>
        <Link href="/" className="mt-4 text-sm text-primary underline">
          Back to inventories
        </Link>
      </div>
    );
  }

  return (
    <div>
      <div className="mb-6">
        <Link
          href={`/inventories/${storageUnit.inventoryId}`}
          className="mb-4 inline-flex items-center text-sm text-muted-foreground hover:text-foreground"
        >
          <ArrowLeft className="mr-1 h-4 w-4" />
          Back to storage units
        </Link>
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-2xl font-bold">{storageUnit.name}</h1>
            <div className="mt-1 flex items-center gap-2">
              <Badge variant="secondary">
                {storageUnit.columns} &times; {storageUnit.rows}
              </Badge>
              <Badge variant="outline">
                {storageUnit.compartmentCount} compartments/bin
              </Badge>
            </div>
          </div>
          <div className="flex gap-2">
            <Button
              variant="outline"
              size="sm"
              onClick={() => setEditOpen(true)}
            >
              <Pencil className="mr-2 h-4 w-4" />
              Edit
            </Button>
            <Button
              variant="destructive"
              size="sm"
              onClick={() => setDeleteOpen(true)}
            >
              <Trash2 className="mr-2 h-4 w-4" />
              Delete
            </Button>
          </div>
        </div>
      </div>

      <StorageUnitGrid
        columns={storageUnit.columns}
        rows={storageUnit.rows}
        bins={storageUnit.bins}
      />

      <Dialog open={editOpen} onOpenChange={setEditOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Edit Storage Unit</DialogTitle>
          </DialogHeader>
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onUpdate)} className="space-y-4">
              <FormField
                control={form.control}
                name="name"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Name</FormLabel>
                    <FormControl>
                      <Input {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <Button
                type="submit"
                className="w-full"
                disabled={updateStorageUnit.isPending}
              >
                {updateStorageUnit.isPending ? "Saving..." : "Save"}
              </Button>
            </form>
          </Form>
        </DialogContent>
      </Dialog>

      <AlertDialog open={deleteOpen} onOpenChange={setDeleteOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Are you sure?</AlertDialogTitle>
            <AlertDialogDescription>
              This action cannot be undone. This will permanently delete the
              storage unit &quot;{storageUnit.name}&quot; and all its bins and
              compartments.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <AlertDialogAction
              onClick={onDelete}
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
            >
              {deleteStorageUnit.isPending ? "Deleting..." : "Delete"}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}
