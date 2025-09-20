"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { api } from "@/lib/api";
import { useState } from "react";

export default function Comments({ cardId }: { cardId: string }) {
  const qc = useQueryClient();
  const [text, setText] = useState("");

  const { data } = useQuery({
    queryKey: ["comments", cardId],
    queryFn: () => api.get(`/cards/${cardId}/comments`).then(r=>r.json())
  });

  const add = useMutation({
    mutationFn: () => api.post(`/cards/${cardId}/comments?body=${encodeURIComponent(text)}`),
    onSuccess: () => {
      setText("");
      qc.invalidateQueries({ queryKey: ["comments", cardId] });
    }
  });

  return (
    <div className="mt-4 border-t pt-4">
      <h4 className="font-semibold mb-2">Discussion</h4>
      <div className="flex gap-2">
        <input className="border rounded px-2 py-1 w-full dark:bg-zinc-800" placeholder="Write a comment…" value={text} onChange={e=>setText(e.target.value)} />
        <button className="px-3 py-1 rounded bg-black text-white" onClick={()=>add.mutate()}>Post</button>
      </div>

      <ul className="mt-3 space-y-2">
        {(data ?? []).map((c:any)=>(
          <li key={c.id} className="border rounded p-2">
            <div className="text-sm">{c.body}</div>
            <div className="text-[10px] text-gray-500">{new Date(c.createdAt).toLocaleString()}</div>
          </li>
        ))}
      </ul>
    </div>
  );
}
