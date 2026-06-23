export interface Metric {
  id: number;
  hostId: number;
  timestamp: string;        // ISO string from the API
  cpu: { details: CpuCore[] };
  memory: { total: number; used: number; free: number; usagePercentage: number };
  disk: { usedPercent: number; free: number; used: number; total: number };
  network: { details: NetworkDevice[] };
}

export interface NetworkDevice { 
    name: string; 
    bytesSent: number;
    bytesReceived: number;
    packetsSent: number;
    packetsReceived: number;
}

export interface CpuCore { 
    id: number; 
    usage: number; 
}

export interface Host {
    id: number;
    hostname: string;
    os: string;
    platform: string;
    platformFamily: string;
    platformVersion: string;
    kernel: string;
    hostId: string;
    bootTime: string;
    uptime: string;
}
