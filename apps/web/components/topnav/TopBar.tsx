"use client";

import Link from "next/link";
import ThemeToggle from "@/components/ui/ThemeToggle";

export default function TopBar() {
  return (
    <header className="border-b bg-white/80 backdrop-blur dark:bg-zinc-900/80 sticky top-0 z-50">
      <div className="max-w-7xl mx-auto px-4 py-3 flex items-center justify-between">
        <Link href="/" className="font-semibold">ShipLife</Link>
        <nav className="flex items-center gap-4 text-sm">
          <Link href="/(board)/board">Board</Link>
          <Link href="/(tools)/analytics">Analytics</Link>
          <Link href="/(tools)/search">Search</Link>
          <Link href="/(tools)/export">Export</Link>
          <ThemeToggle />
        </nav>
      </div>
    </header>
  );
}
