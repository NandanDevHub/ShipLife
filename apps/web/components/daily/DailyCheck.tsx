"use client";

import { useMutation, useQuery } from "@tanstack/react-query";
import { api } from "@/lib/api";
import { useMemo, useState } from "react";

export default function DailyCheck() {
  const todayIso = useMemo(() => new Date().toISOString().slice(0,10), []);
  const [text, setText] = useState("");

  const notes = useQuery({
    queryKey: ["daily"],
    queryFn: () => api.get("/daily").then(r=>r.json())
  });

  const save = useMutation({
    mutationFn: () => api.post("/daily", { date: todayIso, text }),
    onSuccess: () => notes.refetch()
  });

  return (
    <section className="rounded-xl border bg-white dark:bg-zinc-900 p-4 space-y-3">
      <div className="flex items-center justify-between">
        <h2 className="text-lg font-semibold">Daily Check</h2>
        <button className="px-3 py-1 rounded bg-black text-white" onClick={()=>save.mutate()}>Save</button>
      </div>
      <input
        className="border rounded px-3 py-2 w-full dark:bg-zinc-800"
        placeholder="One line for today…"
        value={text}
        onChange={e=>setText(e.target.value)}
      />
      <div>
        <div className="text-xs text-gray-500 mb-1">Recent</div>
        <ul className="text-sm space-y-1">
          {(notes.data ?? []).map((n:any)=>(
            <li key={n.id}>{n.date} — {n.text}</li>
          ))}
        </ul>
      </div>
    </section>
  );
}
