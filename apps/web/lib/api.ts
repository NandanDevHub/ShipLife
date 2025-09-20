const BASE = process.env.NEXT_PUBLIC_API_BASE_URL || "http://localhost:5080";

export const api = {
  get: (path: string, init?: RequestInit) => fetch(`${BASE}${path}`, { ...init, credentials: "include" }),
  post: (path: string, body?: any) =>
    fetch(`${BASE}${path}`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: body ? JSON.stringify(body) : undefined,
      credentials: "include",
    }),
  patch: (path: string, body?: any) =>
    fetch(`${BASE}${path}`, {
      method: "PATCH",
      headers: { "Content-Type": "application/json" },
      body: body ? JSON.stringify(body) : undefined,
      credentials: "include",
    }),
  del: (path: string) =>
    fetch(`${BASE}${path}`, { method: "DELETE", credentials: "include" }),
};
