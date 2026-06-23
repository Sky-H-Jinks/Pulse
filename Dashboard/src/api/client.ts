import type { Host, Metric } from "./types";

const BASE = "http://localhost:5134/api";

export async function getHosts(): Promise<Host[]> {
    const response = await fetch(`${BASE}/hosts`);
    if(!response.ok) throw new Error(`getHosts failed: ${response.status} ${response.statusText}`);
    return response.json();
}

export async function getHostMetrics(hostId: number, from?: string, to?: string): Promise<Metric[]> {
    const url = new URL(`${BASE}/hosts/${hostId}/metrics`);
    if (from) url.searchParams.append("from", from);
    if (to) url.searchParams.append("to", to);
    const response = await fetch(url.toString());
    if(!response.ok) throw new Error(`getHostMetrics failed: ${response.status} ${response.statusText}`);
    return response.json();
}