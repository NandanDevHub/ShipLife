"use client";

import DailyCheck from "@/components/daily/DailyCheck";
import Link from "next/link";

export default function Page() {
  return (
    <main className="space-y-6">
      <section className="rounded-xl border bg-white dark:bg-zinc-900 p-4">
        <h1 className="text-2xl font-semibold">ShipLife</h1>
        <p className="text-sm text-gray-600 dark:text-zinc-400">
          Life-Agile board for seasons (quarters), sprints, and daily momentum.
        </p>
      </section>

      <DailyCheck />

      <div className="grid sm:grid-cols-3 gap-4">
        <Link href="/(board)/board" className="rounded-lg border p-4 bg-white dark:bg-zinc-900">
          <div className="font-medium">Open Board</div>
          <div className="text-sm text-gray-500">Backlog → Ready → Doing → Done</div>
        </Link>
        <Link href="/(tools)/analytics" className="rounded-lg border p-4 bg-white dark:bg-zinc-900">
          <div className="font-medium">Analytics</div>
          <div className="text-sm text-gray-500">Streaks, done, bugs</div>
        </Link>
        <Link href="/(tools)/search" className="rounded-lg border p-4 bg-white dark:bg-zinc-900">
          <div className="font-medium">Search</div>
          <div className="text-sm text-gray-500">Find cards quickly</div>
        </Link>
      </div>
    </main>
  );
}
