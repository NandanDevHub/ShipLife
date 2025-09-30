"use client";

import Board from "@/components/board/Board";
import Planner from "@/components/sprint/Planner";
import Retro from "@/components/sprint/Retro";

export default function BoardPage() {
  return (
    <div className="space-y-6">
      <Board />
      <div className="grid lg:grid-cols-2 gap-6">
        <Planner />
        <Retro />
      </div>
    </div>
  );
}
