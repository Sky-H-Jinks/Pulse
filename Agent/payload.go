package main

import "time"

type Payload struct {
	SystemInfo []SystemInfo `json:"system_info"`
}

type SystemInfo struct {
	TimeStamp time.Time      `json:"timestamp"`
	CPU       CpuPayload     `json:"cpu"`
	Memory    MemoryPayload  `json:"memory"`
	Disk      DiskPayload    `json:"disk"`
	Network   NetworkPayload `json:"network"`
	Host      HostPayload    `json:"host"`
}

type MemoryPayload struct {
	Total       uint64  `json:"total"`
	Used        uint64  `json:"used"`
	Available   uint64  `json:"available"`
	UsedPercent float64 `json:"used_percent"`
	Free        uint64  `json:"free"`
}

type DiskPayload struct {
	Total       uint64  `json:"total"`
	UsedPercent float64 `json:"used_percent"`
	Used        uint64  `json:"used"`
	Free        uint64  `json:"free"`
}

type NetworkPayload struct {
	NetworkDevicePayload []NetworkDevicePayload `json:"network_devices"`
}

type NetworkDevicePayload struct {
	Name        string `json:"name"`
	BytesSent   uint64 `json:"bytes_sent"`
	BytesRecv   uint64 `json:"bytes_recv"`
	PacketsSent uint64 `json:"packets_sent"`
	PacketsRecv uint64 `json:"packets_recv"`
}

type HostPayload struct {
	Hostname        string `json:"hostname"`
	OS              string `json:"os"`
	Platform        string `json:"platform"`
	PlatformFamily  string `json:"platform_family"`
	PlatformVersion string `json:"platform_version"`
	Kernal          string `json:"kernel_version"`
	HostID          string `json:"host_id"`
	BootTime        uint64 `json:"boot_time"`
	Uptime          uint64 `json:"uptime"`
}

type CpuPayload struct {
	CpuData []CpuInfoPayload `json:"cpu_data"`
}

type CpuInfoPayload struct {
	CpuId    int     `json:"cpu_id"`
	CpuUsage float64 `json:"cpu_usage"`
}
