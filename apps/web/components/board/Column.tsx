"use client";

import { Card as TCard, Column as TColumn } from "@/lib/types";
import CardTile from "./CardTile";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { api } from "@/lib/api";
import { useState } from "react";
import { useDroppable } from "@dnd-kit/core";

export default function ColumnView({ column, onOpen }: { column: TColumn; onOpen: (id: string)=>void }) {
  const qc = useQueryClient();
  const [title, setTitle] = useState("");

  const add = useMutation({
    mutationFn: () =>
      api.post("/cards", {
        columnId: column.id,
        title: title || "Untitled",
        type: "feature",
        priority: 2,
        dueAt: null
      }),
    onSuccess: () => {
      setTitle("");
      qc.invalidateQueries({ queryKey: ["board"] });
    },
  });

  const { setNodeRef } = useDroppable({ id: column.id });

  return (
    <div ref={setNodeRef} className="rounded-lg border bg-white dark:bg-zinc-900 p-3">
      <div className="flex items-center justify-between mb-2">
        <h3 className="font-semibold">{column.name}</h3>
        <span className="text-xs text-gray-500">{column.cards.length}</span>
      </div>

      <div className="flex gap-2 mb-2">
        <input
          className="border rounded px-2 py-1 text-sm w-full dark:bg-zinc-800"
          placeholder={`Add to ${column.name}`}
          value={title}
          onChange={(e)=>setTitle(e.target.value)}
        />
        <button onClick={()=>add.mutate()} className="text-sm px-3 py-1 rounded bg-black text-white">Add</button>
      </div>

      <div className="space-y-2 min-h-10">
        {column.cards.sort((a,b)=>a.order-b.order).map((c: TCard) => (
          <CardTile key={c.id} card={c} onOpen={onOpen} />
        ))}
      </div>
    </div>
  );
}
