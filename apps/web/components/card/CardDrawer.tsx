"use client";

import { useEffect, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { api } from "@/lib/api";
import Checklist from "./Checklist";
import Comments from "./Comments";
import Attachments from "./Attachments";

export default function CardDrawer({ cardId, onClose }: { cardId: string | null; onClose: ()=>void }) {
  const open = !!cardId;
  const qc = useQueryClient();

  const { data } = useQuery({
    enabled: open,
    queryKey: ["card", cardId],
    queryFn: async () => {
      // fetch board and find card for minimal API surface
      const b = await api.get("/boards/current").then(r=>r.json());
      const card = b.columns.flatMap((c:any)=>c.cards).find((c:any)=>c.id === cardId);
      return card;
    }
  });

  const [title, setTitle] = useState("");
  const [type, setType] = useState("feature");
  const [priority, setPriority] = useState(2);
  const [desc, setDesc] = useState("");

  useEffect(()=>{
    if (data) {
      setTitle(data.title ?? "");
      setType(data.type ?? "feature");
      setPriority(data.priority ?? 2);
      setDesc(data.description ?? "");
    }
  }, [data]);

  const save = useMutation({
    mutationFn: () => api.patch(`/cards/${cardId}`, { title, description: desc, type, priority }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ["board"] });
      qc.invalidateQueries({ queryKey: ["card", cardId] });
    }
  });

  if (!open) return null;

  return (
    <div className="fixed inset-0 bg-black/30 z-50" onClick={onClose}>
      <div className="absolute right-0 top-0 bottom-0 w-full sm:w-[540px] bg-white dark:bg-zinc-950 border-l p-4 overflow-y-auto" onClick={e=>e.stopPropagation()}>
        <div className="flex items-center justify-between">
          <h3 className="font-semibold">Card</h3>
          <button className="text-sm underline" onClick={onClose}>Close</button>
        </div>

        <div className="space-y-3 mt-4">
          <input className="border rounded px-2 py-1 w-full dark:bg-zinc-800" value={title} onChange={e=>setTitle(e.target.value)} />
          <div className="flex gap-2">
            <select className="border rounded px-2 py-1 dark:bg-zinc-800" value={type} onChange={e=>setType(e.target.value)}>
              <option value="feature">feature</option>
              <option value="bug">bug</option>
              <option value="chore">chore</option>
              <option value="urgent">urgent</option>
            </select>
            <select className="border rounded px-2 py-1 dark:bg-zinc-800" value={priority} onChange={e=>setPriority(Number(e.target.value))}>
              <option value={1}>low</option>
              <option value={2}>medium</option>
              <option value={3}>high</option>
            </select>
          </div>
          <textarea className="border rounded px-2 py-1 w-full min-h-[120px] dark:bg-zinc-800" value={desc} onChange={e=>setDesc(e.target.value)} placeholder="Description…" />
          <button className="px-3 py-2 rounded bg-black text-white" onClick={()=>save.mutate()}>Save</button>

          <Attachments cardId={cardId!} />
          <Checklist cardId={cardId!} />
          <Comments cardId={cardId!} />
        </div>
      </div>
    </div>
  );
}
