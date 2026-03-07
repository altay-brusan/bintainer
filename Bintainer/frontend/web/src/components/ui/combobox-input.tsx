"use client";

import * as React from "react";
import { cn } from "@/lib/utils";
import { Input } from "@/components/ui/input";
import { Plus } from "lucide-react";

interface ComboboxInputProps {
  value: string;
  onChange: (value: string) => void;
  options: string[];
  placeholder?: string;
  className?: string;
}

export function ComboboxInput({
  value,
  onChange,
  options,
  placeholder,
  className,
}: ComboboxInputProps) {
  const [open, setOpen] = React.useState(false);
  const [focused, setFocused] = React.useState(false);
  const wrapperRef = React.useRef<HTMLDivElement>(null);

  const filtered = React.useMemo(() => {
    if (!value.trim()) return options;
    return options.filter((o) =>
      o.toLowerCase().includes(value.toLowerCase())
    );
  }, [value, options]);

  const exactMatch = options.some(
    (o) => o.toLowerCase() === value.trim().toLowerCase()
  );
  const showCreate = value.trim() && !exactMatch;

  React.useEffect(() => {
    const handleClickOutside = (e: MouseEvent) => {
      if (wrapperRef.current && !wrapperRef.current.contains(e.target as Node)) {
        setOpen(false);
      }
    };
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  const showDropdown = open && focused && (filtered.length > 0 || showCreate);

  return (
    <div ref={wrapperRef} className={cn("relative", className)}>
      <Input
        value={value}
        onChange={(e) => {
          onChange(e.target.value);
          setOpen(true);
        }}
        onFocus={() => {
          setFocused(true);
          setOpen(true);
        }}
        onBlur={() => setFocused(false)}
        placeholder={placeholder}
        onKeyDown={(e) => {
          if (e.key === "Escape") setOpen(false);
        }}
      />
      {showDropdown && (
        <div className="absolute top-full left-0 z-50 mt-1 w-full rounded-md border bg-popover shadow-md">
          <div className="max-h-48 overflow-y-auto p-1">
            {filtered.map((option) => (
              <button
                key={option}
                type="button"
                className={cn(
                  "flex w-full items-center rounded-sm px-2 py-1.5 text-sm outline-none transition-colors hover:bg-accent hover:text-accent-foreground",
                  value.toLowerCase() === option.toLowerCase() &&
                    "bg-accent text-accent-foreground"
                )}
                onMouseDown={(e) => {
                  e.preventDefault();
                  onChange(option);
                  setOpen(false);
                }}
              >
                {option}
              </button>
            ))}
            {showCreate && (
              <button
                type="button"
                className="flex w-full items-center gap-2 rounded-sm px-2 py-1.5 text-sm text-primary outline-none transition-colors hover:bg-accent"
                onMouseDown={(e) => {
                  e.preventDefault();
                  setOpen(false);
                }}
              >
                <Plus className="h-3.5 w-3.5" />
                Create &quot;{value.trim()}&quot;
              </button>
            )}
          </div>
        </div>
      )}
    </div>
  );
}
