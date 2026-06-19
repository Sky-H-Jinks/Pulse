# agent

A lightweight host metrics collector, written in Go. Runs as a single static binary on any machine you want to monitor — gathers CPU, memory, and disk metrics and ships them to the ingest service.

## Why Go

Agents run on other people's machines, so the constraints favour Go: a single static binary with no runtime to install, trivial cross-compilation (one command builds the Raspberry Pi binary from a desktop), a small memory footprint, and goroutines for running collectors concurrently. It's the same reasoning behind the Datadog agent, Prometheus node_exporter, and Telegraf.

## Metric payload

The agent sends batches to ingest as JSON over HTTPS. This shape is the contract shared with the ingest and intelligence services:

```json
{
  "agent_id": "uuid-string",
  "host": "pi-5",
  "sent_at": "2026-06-13T10:15:00Z",
  "metrics": [
    { "name": "cpu.percent",       "value": 23.4,  "at": "2026-06-13T10:14:55Z" },
    { "name": "mem.used_bytes",    "value": 3.1e9, "at": "2026-06-13T10:14:55Z" },
    { "name": "disk.used_percent", "value": 71.2,  "at": "2026-06-13T10:14:55Z", "labels": { "mount": "/" } }
  ]
}
```

## Built with

- [gopsutil](https://github.com/shirou/gopsutil) — cross-platform system metrics
- Go standard library for HTTP and JSON

## Running locally

```bash
go run . --interval 15s --host $(hostname)
```

Configuration is via flags and environment variables: server URL, API key, collection interval, and hostname override.

## Building

```bash
go build -o agent .

# Cross-compile for a Raspberry Pi
GOOS=linux GOARCH=arm64 go build -o agent-arm64 .
```
