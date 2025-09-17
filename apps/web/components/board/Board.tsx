"use client";

import { useEffect, useMemo, useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { api } from "@/lib/api";
import ColumnView from "./Column";
import { Board as TBoard, Column as TColumn, Card as TCard } from "@/lib/types";
import { DndContext, DragEndEvent } from "@dnd-kit/core";
import { arrayMove, SortableContext, verticalListSortingStrategy } from "@dnd-kit/sortable";
import CardDrawer from "@/components/card/CardDrawer";

export default function Board() {
  const qc = useQueryClient();
  const { data, refetch } = useQuery({
    queryKey: ["board"],
    queryFn: async () => {
      const r = await api.get("/boards/current");
      if (r.status === 404) {
        // Seed if none
        await api.post("/boards/seed");
        return (await api.get("/boards/current")).json();
      }
      return r.json();
    },
  });

  const board = data as TBoard | undefined;

  const [activeCardId, setActiveCardId] = useState<string | null>(null);

  const move = useMutation({
    mutationFn: (payload: { cardId: string; toColumnId: string; newOrder: number }) =>
      api.post("/cards/move", payload),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["board"] }),
  });

  function onDragEnd(e: DragEndEvent) {
    const cardId = e.active.id as string;
    const toColumnId = e.over?.id as string | undefined;
    if (!cardId || !toColumnId) return;

    // naive: put on top (0)
    move.mutate({ cardId, toColumnId, newOrder: 0 });
  }

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h2 className="text-xl font-semibold">Quarter Board</h2>
        <button
          className="px-3 py-2 rounded bg-black text-white"
          onClick={() => refetch()}
        >
          Refresh
        </button>
      </div>

      {!board ? (
        <div className="text-sm">Loading…</div>
      ) : (
        <DndContext onDragEnd={onDragEnd}>
          <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
            {board.columns.sort((a,b)=>a.order-b.order).map((col: TColumn) => (
              <SortableContext key={col.id} items={col.cards.map(c=>c.id)} strategy={verticalListSortingStrategy}>
                <ColumnView column={col} onOpen={setActiveCardId} />
              </SortableContext>
            ))}
          </div>
        </DndContext>
      )}

      <CardDrawer cardId={activeCardId} onClose={() => setActiveCardId(null)} />
    </div>
  );
}
