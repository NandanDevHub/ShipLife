import "./globals.css";
import { ReactNode } from "react";
import { ThemeProvider } from "next-themes";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import TopBar from "@/components/topnav/TopBar";

const qc = new QueryClient();

export default function RootLayout({ children }: { children: ReactNode }) {
  return (
    <html lang="en" suppressHydrationWarning>
      <body>
        <ThemeProvider attribute="class" defaultTheme="light" enableSystem>
          <QueryClientProvider client={qc}>
            <TopBar />
            <div className="max-w-7xl mx-auto px-4 py-6">{children}</div>
          </QueryClientProvider>
        </ThemeProvider>
      </body>
    </html>
  );
}
