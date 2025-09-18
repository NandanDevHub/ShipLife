"use client";

import { Card as TCard } from "@/lib/types";
import { useSortable } from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";

export default function CardTile({ card, onOpen }: { card: TCard; onOpen: (id: string)=>void }) {
  const { attributes, listeners, setNodeRef, transform, transition } = useSortable({ id: card.id });
  const style = { transform: CSS.Transform.toString(transform), transition };

  const typeColor = {
    feature: "bg-emerald-100 text-emerald-700",
    bug: "bg-rose-100 text-rose-700",
    chore: "bg-slate-100 text-slate-700",
    urgent: "bg-amber-100 text-amber-700",
  }[card.type];

  return (
    <div ref={setNodeRef} style={style} {...attributes} {...listeners}
      className="border rounded p-2 bg-white dark:bg-zinc-800 hover:bg-zinc-100 dark:hover:bg-zinc-700 cursor-grab">
      <div className="text-sm font-medium">{card.title}</div>
      <div className="text-[10px] uppercase mt-1 inline-block px-1.5 py-0.5 rounded {typeColor}">
        {card.type}
      </div>
      <button className="text-xs underline ml-2" onClick={()=>onOpen(card.id)}>Open</button>
    </div>
  );
}
