import * as XLSX from "xlsx";

export interface BomLine {
  partNumber: string;
  quantity: number;
  reference?: string;
}

const PART_NUMBER_HEADERS = [
  "part number",
  "partnumber",
  "p/n",
  "pn",
  "mpn",
  "manufacturer part number",
  "mfr part",
  "mfr. part",
  "component",
  "value",
];

const QUANTITY_HEADERS = [
  "quantity",
  "qty",
  "count",
  "amount",
  "num",
];

const REFERENCE_HEADERS = [
  "reference",
  "ref",
  "designator",
  "ref des",
  "refdes",
  "references",
];

function findColumn(headers: string[], candidates: string[]): number {
  for (const candidate of candidates) {
    const idx = headers.findIndex(
      (h) => h.toLowerCase().trim() === candidate
    );
    if (idx !== -1) return idx;
  }
  // Partial match fallback
  for (const candidate of candidates) {
    const idx = headers.findIndex((h) =>
      h.toLowerCase().trim().includes(candidate)
    );
    if (idx !== -1) return idx;
  }
  return -1;
}

export async function parseBomFile(file: File): Promise<BomLine[]> {
  const buffer = await file.arrayBuffer();
  const workbook = XLSX.read(buffer, { type: "array" });
  const sheetName = workbook.SheetNames[0];
  const sheet = workbook.Sheets[sheetName];
  const rows = XLSX.utils.sheet_to_json<string[]>(sheet, { header: 1 });

  if (rows.length < 2) return [];

  const headers = (rows[0] ?? []).map((h) => String(h ?? ""));
  const partCol = findColumn(headers, PART_NUMBER_HEADERS);
  const qtyCol = findColumn(headers, QUANTITY_HEADERS);
  const refCol = findColumn(headers, REFERENCE_HEADERS);

  if (partCol === -1) {
    throw new Error(
      "Could not find a part number column. Expected headers like: Part Number, MPN, P/N"
    );
  }

  const lines: BomLine[] = [];
  for (let i = 1; i < rows.length; i++) {
    const row = rows[i];
    if (!row || row.length === 0) continue;

    const partNumber = String(row[partCol] ?? "").trim();
    if (!partNumber) continue;

    const rawQty = qtyCol !== -1 ? row[qtyCol] : undefined;
    const quantity = rawQty ? Math.max(1, Math.round(Number(rawQty) || 1)) : 1;
    const reference =
      refCol !== -1 ? String(row[refCol] ?? "").trim() || undefined : undefined;

    lines.push({ partNumber, quantity, reference });
  }

  return lines;
}
