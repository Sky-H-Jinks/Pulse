# ingest

The metric ingestion service, written in Rust. Receives metric batches from agents over HTTPS, validates them, and writes them to Postgres in bulk. Built for the hot write path, where throughput and predictable latency matter.

## Why Rust

Ingestion is the one place in the system where the write rate is the point. Rust suits it: no garbage-collection pauses on a latency-sensitive path, validation that's effectively free via `serde`, memory safety for a long-running internet-facing service, and async concurrency to handle many agents at once on modest hardware.

## What it does

- Accepts `POST /api/v1/ingest` carrying the [agent metric payload](../agent/README.md#metric-payload)
- Validates the payload and rejects malformed batches
- Authenticates agents via an API key issued by the control-plane
- Buffers and bulk-inserts metrics into Postgres
- Applies backpressure (returns `429`) rather than falling over under load
- Exposes `/healthz`

It does **not** own the database schema — the control-plane owns that via migrations. Ingest only writes to the `metrics` table.

## Built with

- [axum](https://github.com/tokio-rs/axum) — web framework
- [tokio](https://tokio.rs) — async runtime
- [serde](https://serde.rs) — deserialization + validation
- [sqlx](https://github.com/launchbadge/sqlx) — async Postgres with compile-time-checked queries
- [tracing](https://github.com/tokio-rs/tracing) — structured logging

> Vet current crate versions before adding — the Rust ecosystem moves quickly.

## Running locally

```bash
# Postgres must be up first (see repo root)
DATABASE_URL=postgres://pulse:pulse_dev@localhost/pulse cargo run
```

Listens on port `8080` by default.

## Configuration

| Variable | Purpose |
|---|---|
| `DATABASE_URL` | Postgres connection string |
| `INGEST_PORT` | Listen port (default 8080) |
| `RUST_LOG` | Log level, e.g. `ingest=debug,tower_http=info` |