package main

import (
	"encoding/json"
	"fmt"
	"log"
	"time"

	"github.com/shirou/gopsutil/v4/cpu"
	"github.com/shirou/gopsutil/v4/disk"
	"github.com/shirou/gopsutil/v4/host"
	"github.com/shirou/gopsutil/v4/mem"
	"github.com/shirou/gopsutil/v4/net"
)

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

func collectMemoryInfo() (MemoryPayload, error) {
	m, memErr := mem.VirtualMemory()
	if memErr != nil {
		return MemoryPayload{}, fmt.Errorf("reading memory: %w", memErr)
	}
	return MemoryPayload{
		m.Total,
		m.Used,
		m.Available,
		m.UsedPercent,
		m.Free,
	}, nil
}

func collectDiskInfo() (DiskPayload, error) {
	d, diskErr := disk.Usage("/")
	if diskErr != nil {
		return DiskPayload{}, fmt.Errorf("reading disk: %w", diskErr)
	}
	return DiskPayload{
		Total:       d.Total,
		UsedPercent: d.UsedPercent,
		Used:        d.Used,
		Free:        d.Free,
	}, nil
}

func collectNetworkInfo() (NetworkPayload, error) {
	n, netErr := net.IOCounters(true)
	if netErr != nil {
		return NetworkPayload{}, fmt.Errorf("reading network: %w", netErr)
	}

	devices := make([]NetworkDevicePayload, 0, len(n))

	for _, nface := range n {
		devices = append(devices, NetworkDevicePayload{
			Name:        nface.Name,
			BytesSent:   nface.BytesSent,
			BytesRecv:   nface.BytesRecv,
			PacketsSent: nface.PacketsSent,
			PacketsRecv: nface.PacketsRecv,
		})
	}
	return NetworkPayload{NetworkDevicePayload: devices}, nil
}

func collectHostInfo() (HostPayload, error) {
	h, hostErr := host.Info()
	if hostErr != nil {
		return HostPayload{}, fmt.Errorf("reading host: %w", hostErr)
	}
	return HostPayload{
		Hostname:        h.Hostname,
		OS:              h.OS,
		Platform:        h.Platform,
		PlatformFamily:  h.PlatformFamily,
		PlatformVersion: h.PlatformVersion,
		Kernal:          h.KernelVersion,
		HostID:          h.HostID,
		BootTime:        h.BootTime,
		Uptime:          h.Uptime,
	}, nil
}

func collectCpuInfo() (CpuPayload, error) {
	c, cpuErr := cpu.Percent(time.Second, true)
	if cpuErr != nil {
		return CpuPayload{}, fmt.Errorf("reading CPU: %w", cpuErr)
	}

	cpuData := make([]CpuInfoPayload, 0, len(c))

	for i, cface := range c {
		cpuData = append(cpuData, CpuInfoPayload{CpuId: i, CpuUsage: cface})
	}
	return CpuPayload{CpuData: cpuData}, nil
}

func main() {
	memPayload, memErr := collectMemoryInfo()
	if memErr != nil {
		log.Printf("error collecting memory info: %v", memErr)
	}

	diskPayload, diskErr := collectDiskInfo()
	if diskErr != nil {
		log.Printf("error collecting disk info: %v", diskErr)
	}

	networkPayload, netErr := collectNetworkInfo()
	if netErr != nil {
		log.Printf("error collecting network info: %v", netErr)
	}

	hostPayload, hostErr := collectHostInfo()
	if hostErr != nil {
		log.Printf("error collecting host info: %v", hostErr)
	}

	cpuPayload, cpuErr := collectCpuInfo()
	if cpuErr != nil {
		log.Printf("error collecting CPU info: %v", cpuErr)
	}

	sysInfoPayload := SystemInfo{
		TimeStamp: time.Now(),
		Memory:    memPayload,
		Disk:      diskPayload,
		Network:   networkPayload,
		Host:      hostPayload,
		CPU:       cpuPayload,
	}

	payload := Payload{
		SystemInfo: []SystemInfo{sysInfoPayload},
	}

	jsonData, err := json.Marshal(payload)

	if err != nil {
		log.Fatalf("Error marshaling JSON: %v", err)
	}

	fmt.Printf("Payload: %+v\n", string(jsonData))
}
