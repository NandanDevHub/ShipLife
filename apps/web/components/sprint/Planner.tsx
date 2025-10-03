"use client";

import { useMutation, useQuery } from "@tanstack/react-query";
import { api } from "@/lib/api";
import { useState } from "react";

export default function Planner() {
  const { data: board } = useQuery({ queryKey:["board"], queryFn: ()=>api.get("/boards/current").then(r=>r.json()) });
  const { data: daily } = useQuery({ queryKey:["daily"], queryFn: ()=>api.get("/daily").then(r=>r.json()) });

  const [goal, setGoal] = useState("");
  const [reward, setReward] = useState("");

  const start = useMutation({
    mutationFn: async () => {
      const now = new Date(); const in14 = new Date(Date.now()+14*86400000);
      // ensure quarter exists via board
      const qId = board?.quarter?.id ?? board?.quarterId;
      if (!qId) return;
      await api.post("/plan/sprint", {
        quarterId: qId,
        number: 1,
        goal,
        startDate: now.toISOString(),
        endDate: in14.toISOString(),
        reward: reward || null
      });
    }
  });

  return (
    <div className="rounded-xl border bg-white dark:bg-zinc-900 p-4 space-y-3">
      <h3 className="font-semibold">Sprint Planner</h3>
      <input className="border rounded px-2 py-1 w-full dark:bg-zinc-800" placeholder="Sprint goal" value={goal} onChange={e=>setGoal(e.target.value)} />
      <input className="border rounded px-2 py-1 w-full dark:bg-zinc-800" placeholder="Reward (optional)" value={reward} onChange={e=>setReward(e.target.value)} />
      <button className="px-3 py-1 rounded bg-black text-white" onClick={()=>start.mutate()}>Start Sprint</button>
    </div>
  );
}
