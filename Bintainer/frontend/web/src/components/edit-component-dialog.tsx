"use client";

import { useRef, useState, useEffect } from "react";
import {
  Plus,
  Trash2,
  Link as LinkIcon,
  Package,
  ChevronDown,
  Pencil,
  X,
} from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Badge } from "@/components/ui/badge";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Separator } from "@/components/ui/separator";
import {
  Dialog,
  DialogContent,
  DialogTitle,
} from "@/components/ui/dialog";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSub,
  DropdownMenuSubContent,
  DropdownMenuSubTrigger,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { ComboboxInput } from "@/components/ui/combobox-input";
import type { PartAttribute } from "@/types/api";
import type { Component } from "@/lib/demo-data";
import { toast } from "sonner";

const existingFootprints = [
  "0201", "0402", "0603", "0805", "1206", "1210", "2512",
  "SOT-23", "SOT-223", "SOT-89",
  "TO-92", "TO-220", "TO-252",
  "SOP-8", "SOIC-8", "SOIC-16",
  "TSSOP-8", "TSSOP-16", "TSSOP-20",
  "QFP-32", "QFP-44", "QFP-48", "QFP-64",
  "LQFP-32", "LQFP-48", "LQFP-64", "LQFP-100",
  "QFN-16", "QFN-24", "QFN-32", "QFN-48", "QFN-56",
  "BGA-256", "BGA-484",
  "DIP-8", "DIP-14", "DIP-16", "DIP-28",
  "SMA", "SMB", "SMC",
];

const categoryTree = [
  {
    label: "Resistors",
    children: [
      { label: "Chip Resistor (SMD)" },
      { label: "Through Hole" },
      { label: "Potentiometers" },
      { label: "Thermistors" },
    ],
  },
  {
    label: "Capacitors",
    children: [
      { label: "Ceramic (SMD)" },
      { label: "Electrolytic" },
      { label: "Film" },
      { label: "Tantalum" },
    ],
  },
  {
    label: "Inductors",
    children: [
      { label: "Fixed" },
      { label: "Adjustable" },
      { label: "Chokes" },
    ],
  },
  {
    label: "Transistors",
    children: [
      { label: "BJT (NPN)" },
      { label: "BJT (PNP)" },
      { label: "MOSFET" },
      { label: "IGBT" },
    ],
  },
  {
    label: "Microcontrollers",
    children: [
      { label: "ARM" },
      { label: "AVR" },
      { label: "PIC" },
      { label: "RISC-V" },
    ],
  },
  {
    label: "LEDs",
    children: [
      { label: "Standard" },
      { label: "SMD" },
      { label: "High Power" },
    ],
  },
  {
    label: "Connectors",
    children: [
      { label: "Headers" },
      { label: "USB" },
      { label: "Terminal Blocks" },
    ],
  },
  {
    label: "ICs",
    children: [
      { label: "Op-Amps" },
      { label: "Voltage Regulators" },
      { label: "Logic Gates" },
      { label: "Timers" },
    ],
  },
];

interface EditComponentDialogProps {
  component: Component | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function EditComponentDialog({
  component,
  open,
  onOpenChange,
}: EditComponentDialogProps) {
  const fileInputRef = useRef<HTMLInputElement>(null);

  const [partNumber, setPartNumber] = useState("");
  const [mfrPartNumber, setMfrPartNumber] = useState("");
  const [description, setDescription] = useState("");
  const [imagePreview, setImagePreview] = useState<string | null>(null);
  const [imageFile, setImageFile] = useState<File | null>(null);
  const [manufacturer, setManufacturer] = useState("");
  const [detailedDescription, setDetailedDescription] = useState("");
  const [footprint, setFootprint] = useState("");
  const [category, setCategory] = useState("");
  const [supplier, setSupplier] = useState("");
  const [url, setUrl] = useState("");
  const [binLabel, setBinLabel] = useState("");
  const [quantity, setQuantity] = useState(0);
  const [unitPrice, setUnitPrice] = useState("");
  const [lowStockThreshold, setLowStockThreshold] = useState(0);
  const [tags, setTags] = useState<string[]>([]);
  const [tagInput, setTagInput] = useState("");
  const [attributes, setAttributes] = useState<PartAttribute[]>([]);
  const [newAttrTitle, setNewAttrTitle] = useState("");
  const [newAttrValue, setNewAttrValue] = useState("");

  // Populate form when component changes
  useEffect(() => {
    if (!component) return;
    setPartNumber(component.name);
    setMfrPartNumber(component.name);
    setDescription(component.name);
    setCategory(component.category);
    setSupplier(component.supplier ?? "");
    setFootprint(component.package ?? "");
    setBinLabel(component.bin);
    setQuantity(component.quantity);
    setUnitPrice(component.unitPrice != null ? String(component.unitPrice) : "");
    setLowStockThreshold(component.lowStockThreshold);
    setUrl(component.datasheetUrl ?? "");
    setManufacturer("");
    setDetailedDescription("");
    setImagePreview(null);
    setImageFile(null);
    setTags(component.tags ?? []);
    setTagInput("");
    setAttributes([]);
    setNewAttrTitle("");
    setNewAttrValue("");
  }, [component]);

  const handleFileSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;
    setImageFile(file);
    const reader = new FileReader();
    reader.onload = (ev) => setImagePreview(ev.target?.result as string);
    reader.readAsDataURL(file);
  };

  const removeImage = () => {
    setImageFile(null);
    setImagePreview(null);
    if (fileInputRef.current) fileInputRef.current.value = "";
  };

  const addTag = () => {
    const t = tagInput.trim().toLowerCase();
    if (!t || tags.includes(t)) return;
    setTags((prev) => [...prev, t]);
    setTagInput("");
  };

  const removeTag = (tag: string) => {
    setTags((prev) => prev.filter((t) => t !== tag));
  };

  const addAttribute = () => {
    if (!newAttrTitle.trim()) return;
    setAttributes((prev) => [
      ...prev,
      { title: newAttrTitle.trim(), value: newAttrValue.trim() },
    ]);
    setNewAttrTitle("");
    setNewAttrValue("");
  };

  const removeAttribute = (index: number) => {
    setAttributes((prev) => prev.filter((_, i) => i !== index));
  };

  const handleSubmit = () => {
    if (!partNumber.trim() || !mfrPartNumber.trim() || !description.trim()) {
      toast.error("Please fill in all mandatory fields");
      return;
    }
    // TODO: call API
    console.log("Updating component:", component?.id);
    toast.success("Component updated (demo)");
    onOpenChange(false);
  };

  const isMandatoryValid =
    partNumber.trim() && mfrPartNumber.trim() && description.trim();

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-5xl h-[90vh] gap-0 p-0 grid grid-rows-[auto_1fr_auto] overflow-hidden">
        <div className="px-6 pt-6 pb-4">
          <DialogTitle>Edit Component</DialogTitle>
        </div>

        <ScrollArea className="overflow-auto">
          <div className="space-y-8 px-6 py-5">
            {/* Product Info */}
            <section className="space-y-5">
              <h3 className="text-sm font-semibold text-muted-foreground uppercase tracking-wider">
                Product Information
              </h3>

              <div className="grid grid-cols-[140px_1fr] gap-5 items-start">
                <div className="space-y-2">
                  <div
                    className="flex h-[140px] w-[140px] cursor-pointer items-center justify-center rounded-lg border-2 border-dashed bg-muted/50 overflow-hidden transition-colors hover:border-primary/50 hover:bg-muted"
                    onClick={() => fileInputRef.current?.click()}
                  >
                    {imagePreview ? (
                      // eslint-disable-next-line @next/next/no-img-element
                      <img
                        src={imagePreview}
                        alt="Part"
                        className="h-full w-full object-contain"
                      />
                    ) : (
                      <div className="flex flex-col items-center gap-1.5 text-muted-foreground/30">
                        <Package className="h-12 w-12 stroke-[1.2]" />
                        <span className="text-[10px] text-muted-foreground/50">Click to upload</span>
                      </div>
                    )}
                  </div>
                  <input
                    ref={fileInputRef}
                    type="file"
                    accept="image/*"
                    className="hidden"
                    onChange={handleFileSelect}
                  />
                  {imageFile && (
                    <div className="flex items-center justify-between">
                      <span className="truncate text-[11px] text-muted-foreground max-w-[100px]">
                        {imageFile.name}
                      </span>
                      <Button
                        variant="ghost"
                        size="icon"
                        className="h-6 w-6 text-muted-foreground hover:text-destructive"
                        onClick={removeImage}
                      >
                        <Trash2 className="h-3 w-3" />
                      </Button>
                    </div>
                  )}
                  {!imageFile && (
                    <p className="text-[11px] text-muted-foreground">Optional</p>
                  )}
                </div>

                <div className="space-y-3">
                  <div className="grid grid-cols-2 gap-3">
                    <div>
                      <Label className="text-xs">
                        Part Number <span className="text-destructive">*</span>
                      </Label>
                      <Input
                        placeholder="e.g. J50S10K-ND"
                        value={partNumber}
                        onChange={(e) => setPartNumber(e.target.value)}
                        className="mt-1"
                      />
                    </div>
                    <div>
                      <Label className="text-xs">
                        Manufacturer Part Number <span className="text-destructive">*</span>
                      </Label>
                      <Input
                        placeholder="e.g. J50S 10K"
                        value={mfrPartNumber}
                        onChange={(e) => setMfrPartNumber(e.target.value)}
                        className="mt-1"
                      />
                    </div>
                  </div>
                  <div>
                    <Label className="text-xs">
                      Description <span className="text-destructive">*</span>
                    </Label>
                    <Input
                      placeholder="e.g. POT 10K OHM 1.5W WIREWOUND LIN"
                      value={description}
                      onChange={(e) => setDescription(e.target.value)}
                      className="mt-1"
                    />
                  </div>
                  <div>
                    <Label className="text-xs">Detailed Description</Label>
                    <Textarea
                      placeholder="e.g. 10k Ohm 1 Gang Linear..."
                      value={detailedDescription}
                      onChange={(e) => setDetailedDescription(e.target.value)}
                      className="mt-1"
                      rows={2}
                    />
                  </div>
                </div>
              </div>
            </section>

            <Separator />

            {/* Details */}
            <section className="space-y-5">
              <h3 className="text-sm font-semibold text-muted-foreground uppercase tracking-wider">
                Details
              </h3>

              <div className="grid grid-cols-4 gap-3">
                <div>
                  <Label className="text-xs">Manufacturer</Label>
                  <Input
                    placeholder="e.g. Nidec Components"
                    value={manufacturer}
                    onChange={(e) => setManufacturer(e.target.value)}
                    className="mt-1"
                  />
                </div>
                <div>
                  <Label className="text-xs">Supplier</Label>
                  <Input
                    placeholder="e.g. Digikey, Mouser"
                    value={supplier}
                    onChange={(e) => setSupplier(e.target.value)}
                    className="mt-1"
                  />
                </div>
                <div>
                  <Label className="text-xs">Category</Label>
                  <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                      <Button
                        variant="outline"
                        className="mt-1 w-full justify-between font-normal"
                      >
                        <span className={category ? "text-foreground" : "text-muted-foreground"}>
                          {category || "Select category"}
                        </span>
                        <ChevronDown className="h-4 w-4 text-muted-foreground" />
                      </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent className="w-56">
                      <DropdownMenuItem onClick={() => setCategory("")}>
                        <span className="text-muted-foreground">No category</span>
                      </DropdownMenuItem>
                      {categoryTree.map((group) => (
                        <DropdownMenuSub key={group.label}>
                          <DropdownMenuSubTrigger>{group.label}</DropdownMenuSubTrigger>
                          <DropdownMenuSubContent>
                            <DropdownMenuItem onClick={() => setCategory(group.label)}>
                              All {group.label}
                            </DropdownMenuItem>
                            {group.children.map((child) => (
                              <DropdownMenuItem
                                key={child.label}
                                onClick={() => setCategory(`${group.label} > ${child.label}`)}
                              >
                                {child.label}
                              </DropdownMenuItem>
                            ))}
                          </DropdownMenuSubContent>
                        </DropdownMenuSub>
                      ))}
                    </DropdownMenuContent>
                  </DropdownMenu>
                </div>
                <div>
                  <Label className="text-xs">Footprint / Package</Label>
                  <ComboboxInput
                    value={footprint}
                    onChange={setFootprint}
                    options={existingFootprints}
                    placeholder="e.g. 0603, TO-92"
                    className="mt-1"
                  />
                </div>
                <div className="col-span-4">
                  <Label className="text-xs">Product URL / Datasheet</Label>
                  <div className="relative mt-1">
                    <LinkIcon className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
                    <Input
                      placeholder="https://..."
                      value={url}
                      onChange={(e) => setUrl(e.target.value)}
                      className="pl-9"
                    />
                  </div>
                </div>
                <div className="col-span-4">
                  <Label className="text-xs">Tags</Label>
                  <div className="mt-1 flex flex-wrap items-center gap-1.5 rounded-md border bg-background p-2 min-h-[38px]">
                    {tags.map((tag) => (
                      <Badge key={tag} variant="secondary" className="gap-1 text-xs">
                        {tag}
                        <X
                          className="h-3 w-3 cursor-pointer hover:text-destructive"
                          onClick={() => removeTag(tag)}
                        />
                      </Badge>
                    ))}
                    <Input
                      value={tagInput}
                      onChange={(e) => setTagInput(e.target.value)}
                      onKeyDown={(e) => {
                        if (e.key === "Enter") { e.preventDefault(); addTag(); }
                        if (e.key === "Backspace" && !tagInput && tags.length > 0) {
                          setTags((prev) => prev.slice(0, -1));
                        }
                      }}
                      placeholder={tags.length === 0 ? "Type and press Enter to add tags" : "Add tag..."}
                      className="flex-1 min-w-[120px] border-0 p-0 h-6 text-sm shadow-none focus-visible:ring-0"
                    />
                  </div>
                  <p className="mt-1 text-[11px] text-muted-foreground">Optional. Press Enter to add each tag.</p>
                </div>
              </div>
            </section>

            <Separator />

            {/* Storage */}
            <section className="space-y-5">
              <h3 className="text-sm font-semibold text-muted-foreground uppercase tracking-wider">
                Storage
              </h3>

              <div className="grid grid-cols-4 gap-3">
                <div>
                  <Label className="text-xs">Bin Label</Label>
                  <Input
                    placeholder="e.g. Pod"
                    value={binLabel}
                    onChange={(e) => setBinLabel(e.target.value)}
                    className="mt-1"
                  />
                  <p className="mt-1 text-[11px] text-muted-foreground">
                    Label on the physical bin
                  </p>
                </div>
                <div>
                  <Label className="text-xs">Quantity</Label>
                  <Input
                    type="number"
                    min={0}
                    value={quantity}
                    onChange={(e) => setQuantity(Number(e.target.value))}
                    className="mt-1"
                  />
                </div>
                <div>
                  <Label className="text-xs">Unit Price ($)</Label>
                  <Input
                    type="number"
                    min={0}
                    step="0.001"
                    placeholder="0.00"
                    value={unitPrice}
                    onChange={(e) => setUnitPrice(e.target.value)}
                    className="mt-1"
                  />
                </div>
                <div>
                  <Label className="text-xs">Low Stock Threshold</Label>
                  <Input
                    type="number"
                    min={0}
                    value={lowStockThreshold}
                    onChange={(e) => setLowStockThreshold(Number(e.target.value))}
                    className="mt-1"
                  />
                </div>
              </div>
            </section>

            <Separator />

            {/* Dynamic Attributes */}
            <section className="space-y-5">
              <div className="flex items-center justify-between">
                <h3 className="text-sm font-semibold text-muted-foreground uppercase tracking-wider">
                  Product Attributes
                </h3>
                <span className="text-xs text-muted-foreground">
                  {attributes.length} attribute{attributes.length !== 1 ? "s" : ""}
                </span>
              </div>

              {attributes.length > 0 && (
                <div className="rounded-lg border">
                  <div className="grid grid-cols-[1fr_1fr_36px] gap-2 border-b bg-muted/50 px-3 py-2 text-xs font-medium text-muted-foreground">
                    <span>Type</span>
                    <span>Description</span>
                    <span />
                  </div>
                  {attributes.map((attr, i) => (
                    <div
                      key={i}
                      className="grid grid-cols-[1fr_1fr_36px] items-center gap-2 border-b last:border-0 px-3 py-2"
                    >
                      <span className="text-sm font-medium">{attr.title}</span>
                      <span className="text-sm">{attr.value}</span>
                      <Button
                        variant="ghost"
                        size="icon"
                        className="h-7 w-7 text-muted-foreground hover:text-destructive"
                        onClick={() => removeAttribute(i)}
                      >
                        <Trash2 className="h-3.5 w-3.5" />
                      </Button>
                    </div>
                  ))}
                </div>
              )}

              <div className="flex items-end gap-2">
                <div className="flex-1">
                  <Label className="text-xs">Type</Label>
                  <Input
                    placeholder="e.g. Resistance (Ohms)"
                    value={newAttrTitle}
                    onChange={(e) => setNewAttrTitle(e.target.value)}
                    className="mt-1"
                    onKeyDown={(e) => {
                      if (e.key === "Enter") { e.preventDefault(); addAttribute(); }
                    }}
                  />
                </div>
                <div className="flex-1">
                  <Label className="text-xs">Description</Label>
                  <Input
                    placeholder="e.g. 10k"
                    value={newAttrValue}
                    onChange={(e) => setNewAttrValue(e.target.value)}
                    className="mt-1"
                    onKeyDown={(e) => {
                      if (e.key === "Enter") { e.preventDefault(); addAttribute(); }
                    }}
                  />
                </div>
                <Button
                  variant="outline"
                  size="icon"
                  className="h-9 w-9 shrink-0"
                  onClick={addAttribute}
                  disabled={!newAttrTitle.trim()}
                >
                  <Plus className="h-4 w-4" />
                </Button>
              </div>
            </section>
          </div>
        </ScrollArea>

        {/* Footer */}
        <div className="flex items-center justify-between border-t bg-card px-6 py-5">
          <div className="flex items-center gap-2">
            {!isMandatoryValid && (
              <Badge variant="secondary" className="text-sm text-muted-foreground">
                <span className="text-destructive mr-1">*</span> Required fields missing
              </Badge>
            )}
          </div>
          <div className="flex gap-3">
            <Button variant="outline" size="lg" onClick={() => onOpenChange(false)}>
              Cancel
            </Button>
            <Button size="lg" onClick={handleSubmit} disabled={!isMandatoryValid}>
              <Pencil className="mr-2 h-4 w-4" />
              Save Changes
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
}
