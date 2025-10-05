"use client";

import { api } from "@/lib/api";

export default function ExportPage() {
  async function download() {
    const res = await api.get("/export/json");
    const blob = await res.blob();
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = "shiplife-backup.json";
    a.click();
    URL.revokeObjectURL(url);
  }
  return (
    <div className="rounded-xl border bg-white dark:bg-zinc-900 p-4">
      <h2 className="text-xl font-semibold mb-2">Export / Import</h2>
      <button className="px-3 py-2 rounded bg-black text-white" onClick={download}>Export JSON</button>
    </div>
  );
}
