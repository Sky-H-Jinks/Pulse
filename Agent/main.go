package main

import (
	"bytes"
	"context"
	"encoding/json"
	"fmt"
	"log"
	"net/http"
	"os"
	"os/signal"
	"syscall"
	"time"

	"github.com/shirou/gopsutil/v4/cpu"
	"github.com/shirou/gopsutil/v4/disk"
	"github.com/shirou/gopsutil/v4/host"
	"github.com/shirou/gopsutil/v4/mem"
	"github.com/shirou/gopsutil/v4/net"
)

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

func collectAndEmit() {
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

	resp, err := http.Post(
		"http://localhost:8080/api/v1/ingest",
		"application/json",
		bytes.NewReader(jsonData),
	)

	if err != nil {
		log.Printf("error shipping payload: %v", err)
		return
	}
	defer resp.Body.Close()

	if resp.StatusCode != http.StatusCreated {
		log.Printf("ingest returned unexpected status: %d", resp.StatusCode)
	}
}

//func goodbyeAndEmit(ctx context.Context) {
// Collect reason for shutdown and notify server of scheduled shutdown alongside any existing system data collected
//}

func main() {
	ctx, stop := signal.NotifyContext(context.Background(), os.Interrupt, syscall.SIGTERM)
	defer stop()

	ticker := time.NewTicker(1 * time.Second)
	defer ticker.Stop()

	log.Println("Starting system info collection...")
	collectAndEmit()

	for {
		select {
		case <-ctx.Done():
			//goodbyeAndEmit(ctx)
			log.Println("Shutting down system info collection...")
			return
		case <-ticker.C:
			collectAndEmit()
		}
	}
}
