"use client";

import { useMutation, useQuery } from "@tanstack/react-query";
import { api } from "@/lib/api";
import { useState } from "react";

export default function Retro() {
  const { data: board } = useQuery({ queryKey:["board"], queryFn: ()=>api.get("/boards/current").then(r=>r.json()) });
  const [win, setWin] = useState("");
  const [blocker, setBlocker] = useState("");
  const [change, setChange] = useState("");

  const submit = useMutation({
    mutationFn: async () => {
      // retro PATCH to latest sprint; for demo we assume sprint id is first/only
      const sprints = (board?.quarter?.sprints ?? []) as any[];
      const latest = sprints[sprints.length-1];
      if (!latest) { alert("No sprint found. Start one in Planner."); return; }
      await api.patch(`/plan/sprint/${latest.id}/retro?win=${encodeURIComponent(win)}&blocker=${encodeURIComponent(blocker)}&oneChange=${encodeURIComponent(change)}`);
      setWin(""); setBlocker(""); setChange("");
      alert("Retro saved.");
    }
  });

  return (
    <div className="rounded-xl border bg-white dark:bg-zinc-900 p-4 space-y-3">
      <h3 className="font-semibold">Retro</h3>
      <input className="border rounded px-2 py-1 w-full dark:bg-zinc-800" placeholder="Win" value={win} onChange={e=>setWin(e.target.value)} />
      <input className="border rounded px-2 py-1 w-full dark:bg-zinc-800" placeholder="Blocker" value={blocker} onChange={e=>setBlocker(e.target.value)} />
      <input className="border rounded px-2 py-1 w-full dark:bg-zinc-800" placeholder="One change" value={change} onChange={e=>setChange(e.target.value)} />
      <button className="px-3 py-1 rounded bg-black text-white" onClick={()=>submit.mutate()}>Save Retro</button>
    </div>
  );
}
