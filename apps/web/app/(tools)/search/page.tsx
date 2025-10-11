"use client";

import { useQuery } from "@tanstack/react-query";
import { api } from "@/lib/api";
import { useState } from "react";

export default function SearchPage() {
  const [q, setQ] = useState("");
  const { data, refetch } = useQuery({
    queryKey: ["search", q],
    queryFn: () => api.get(`/search?q=${encodeURIComponent(q)}`).then(r => r.json()),
  });

  return (
    <div className="rounded-xl border bg-white dark:bg-zinc-900 p-4 space-y-3">
      <h2 className="text-xl font-semibold">Search</h2>
      <div className="flex gap-2">
        <input className="border rounded px-3 py-2 w-full dark:bg-zinc-800" placeholder="Search title or description…" value={q} onChange={e=>setQ(e.target.value)} />
        <button className="px-3 py-2 rounded bg-black text-white" onClick={()=>refetch()}>Search</button>
      </div>
      <ul className="divide-y">
        {(data ?? []).map((c: any) => (
          <li key={c.id} className="py-2">{c.title}</li>
        ))}
      </ul>
    </div>
  );
}
