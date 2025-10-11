"use client";

import { useQuery } from "@tanstack/react-query";
import { api } from "@/lib/api";

export default function AnalyticsPage() {
  const { data } = useQuery({
    queryKey: ["analytics"],
    queryFn: () => api.get("/analytics/summary").then(r => r.json())
  });

  return (
    <div className="rounded-xl border bg-white dark:bg-zinc-900 p-4">
      <h2 className="text-xl font-semibold mb-2">Analytics</h2>
      <pre className="text-sm whitespace-pre-wrap">
        {JSON.stringify(data ?? {}, null, 2)}
      </pre>
    </div>
  );
}
