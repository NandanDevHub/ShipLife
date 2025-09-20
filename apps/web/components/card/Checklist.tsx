"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { api } from "@/lib/api";
import { useState } from "react";

export default function Checklist({ cardId }: { cardId: string }) {
  const qc = useQueryClient();
  const [title, setTitle] = useState("");

  const { data } = useQuery({
    queryKey: ["card-checklists", cardId],
    queryFn: async () => {
      // use board fetch to avoid extra endpoints; filter items for the cardId
      const b = await api.get("/boards/current").then(r=>r.json());
      return b.columns.flatMap((c:any)=>c.cards).find((c:any)=>c.id===cardId)?.checklists ?? [];
    }
  });

  const addChecklist = useMutation({
    mutationFn: () => api.post(`/cards/${cardId}/checklists?title=${encodeURIComponent(title || "Checklist")}`),
    onSuccess: () => {
      setTitle("");
      qc.invalidateQueries({ queryKey: ["board"] });
      qc.invalidateQueries({ queryKey: ["card-checklists", cardId] });
    }
  });

  async function toggleItem(id: string, done: boolean) {
    await api.patch(`/cards/checklist-items/${id}?done=${done}`);
    qc.invalidateQueries({ queryKey: ["board"] });
    qc.invalidateQueries({ queryKey: ["card-checklists", cardId] });
  }

  async function addItem(cid: string) {
    const text = prompt("Checklist item")?.trim();
    if (!text) return;
    await api.post(`/cards/checklists/${cid}/items?text=${encodeURIComponent(text)}`);
    qc.invalidateQueries({ queryKey: ["board"] });
    qc.invalidateQueries({ queryKey: ["card-checklists", cardId] });
  }

  return (
    <div className="mt-4 border-t pt-4">
      <div className="flex items-center justify-between">
        <h4 className="font-semibold">Checklist</h4>
        <div className="flex gap-2">
          <input className="border rounded px-2 py-1 text-sm dark:bg-zinc-800" placeholder="Checklist title" value={title} onChange={e=>setTitle(e.target.value)} />
          <button className="text-sm px-3 py-1 rounded bg-black text-white" onClick={()=>addChecklist.mutate()}>Add</button>
        </div>
      </div>

      <div className="space-y-3 mt-2">
        {(data ?? []).map((cl: any)=>(
          <div key={cl.id} className="rounded border p-2">
            <div className="font-medium text-sm">{cl.title}</div>
            <ul className="mt-2 space-y-1">
              {(cl.items ?? []).map((it: any)=>(
                <li key={it.id} className="flex items-center gap-2">
                  <input type="checkbox" checked={it.done} onChange={e=>toggleItem(it.id, e.target.checked)} />
                  <span className={it.done ? "line-through text-gray-500" : ""}>{it.text}</span>
                </li>
              ))}
            </ul>
            <button className="text-xs underline mt-1" onClick={()=>addItem(cl.id)}>+ Add item</button>
          </div>
        ))}
      </div>
    </div>
  );
}
