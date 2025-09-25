"use client";

import { ReactNode } from "react";

export default function Shell({
  title,
  subtitle,
  children,
}: {
  title?: string;
  subtitle?: string;
  children: ReactNode;
}) {
  return (
    <section className="rounded-xl border bg-white dark:bg-zinc-900 p-4 space-y-2">
      {(title || subtitle) && (
        <header>
          {title && <h2 className="text-xl font-semibold">{title}</h2>}
          {subtitle && (
            <p className="text-sm text-gray-600 dark:text-zinc-400">{subtitle}</p>
          )}
        </header>
      )}
      <div>{children}</div>
    </section>
  );
}
