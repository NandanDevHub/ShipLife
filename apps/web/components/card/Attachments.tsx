"use client";

import { api } from "@/lib/api";
import { useQuery, useQueryClient } from "@tanstack/react-query";
import { useRef, useState } from "react";

export default function Attachments({ cardId }: { cardId: string }) {
  const inputRef = useRef<HTMLInputElement | null>(null);
  const qc = useQueryClient();
  const [busy, setBusy] = useState(false);

  const { data } = useQuery({
    queryKey: ["attachments", cardId],
    queryFn: async () => {
      const b = await api.get("/boards/current").then(r=>r.json());
      const card = b.columns.flatMap((c:any)=>c.cards).find((c:any)=>c.id===cardId);
      return card?.attachments ?? [];
    }
  });

  async function upload(e: React.ChangeEvent<HTMLInputElement>) {
    const file = e.target.files?.[0];
    if (!file) return;
    setBusy(true);
    const fd = new FormData();
    fd.append("file", file);
    const res = await fetch(`${process.env.NEXT_PUBLIC_API_BASE_URL || "http://localhost:5080"}/attachments/upload/${cardId}`, {
      method: "POST",
      body: fd,
    });
    setBusy(false);
    if (!res.ok) { alert("Upload failed"); return; }
    qc.invalidateQueries({ queryKey: ["attachments", cardId] });
    qc.invalidateQueries({ queryKey: ["board"] });
    if (inputRef.current) inputRef.current.value = "";
  }

  return (
    <div className="mt-4 border-t pt-4">
      <h4 className="font-semibold mb-2">Attachments</h4>
      <input ref={inputRef} type="file" onChange={upload} disabled={busy} />
      <ul className="mt-2 space-y-1">
        {(data ?? []).map((a:any)=>(
          <li key={a.id}>
            <a className="text-sm underline" href={a.url} target="_blank">{a.fileName}</a>
          </li>
        ))}
      </ul>
    </div>
  );
}
