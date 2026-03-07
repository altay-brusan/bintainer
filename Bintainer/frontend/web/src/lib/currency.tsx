"use client";

import { createContext, useContext, useState, useCallback, type ReactNode } from "react";

export interface CurrencyInfo {
  code: string;
  symbol: string;
  name: string;
}

export const CURRENCIES: CurrencyInfo[] = [
  { code: "USD", symbol: "$", name: "US Dollar" },
  { code: "EUR", symbol: "\u20AC", name: "Euro" },
  { code: "GBP", symbol: "\u00A3", name: "British Pound" },
  { code: "JPY", symbol: "\u00A5", name: "Japanese Yen" },
  { code: "CNY", symbol: "\u00A5", name: "Chinese Yuan" },
  { code: "KRW", symbol: "\u20A9", name: "South Korean Won" },
  { code: "TRY", symbol: "\u20BA", name: "Turkish Lira" },
  { code: "INR", symbol: "\u20B9", name: "Indian Rupee" },
  { code: "CAD", symbol: "CA$", name: "Canadian Dollar" },
  { code: "AUD", symbol: "A$", name: "Australian Dollar" },
  { code: "CHF", symbol: "CHF", name: "Swiss Franc" },
  { code: "SEK", symbol: "kr", name: "Swedish Krona" },
];

interface CurrencyContextValue {
  currency: CurrencyInfo;
  setCurrencyCode: (code: string) => void;
  format: (amount: number, decimals?: number) => string;
}

const CurrencyContext = createContext<CurrencyContextValue | null>(null);

function getInitialCurrency(): CurrencyInfo {
  if (typeof window !== "undefined") {
    const saved = localStorage.getItem("bintainer-currency");
    if (saved) {
      const found = CURRENCIES.find((c) => c.code === saved);
      if (found) return found;
    }
  }
  return CURRENCIES[0]; // USD
}

export function CurrencyProvider({ children }: { children: ReactNode }) {
  const [currency, setCurrency] = useState<CurrencyInfo>(getInitialCurrency);

  const setCurrencyCode = useCallback((code: string) => {
    const found = CURRENCIES.find((c) => c.code === code);
    if (found) {
      setCurrency(found);
      localStorage.setItem("bintainer-currency", code);
    }
  }, []);

  const format = useCallback(
    (amount: number, decimals?: number) => {
      const d = decimals ?? (currency.code === "JPY" || currency.code === "KRW" ? 0 : 2);
      return `${currency.symbol}${amount.toFixed(d)}`;
    },
    [currency]
  );

  return (
    <CurrencyContext.Provider value={{ currency, setCurrencyCode, format }}>
      {children}
    </CurrencyContext.Provider>
  );
}

export function useCurrency() {
  const ctx = useContext(CurrencyContext);
  if (!ctx) throw new Error("useCurrency must be used within CurrencyProvider");
  return ctx;
}
