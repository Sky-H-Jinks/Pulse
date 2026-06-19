# control-plane

The core service, written in C# on ASP.NET Core. Owns the business logic of the platform: what to monitor, when something counts as down, who gets alerted, and pushing live state to the dashboard. It also owns the database schema.

## Why C#

This is the feature-dense, long-lived service — CRUD APIs, a check scheduler, an incident state machine, notification dispatch, realtime push, and database ownership. ASP.NET Core fits that shape well: first-class dependency injection, EF Core migrations as the schema source of truth, `BackgroundService` for scheduling, and SignalR for realtime without a separate websocket layer.

## What it does

- Monitor definitions (HTTP/TCP uptime checks)
- A scheduler that runs due checks and records results
- Incident lifecycle: open on consecutive failures, close on recovery
- Alert rules and notification dispatch (webhooks, later Slack/email)
- A SignalR hub broadcasting live check and incident updates
- API key issuance for agents
- Owns the Postgres schema via EF Core migrations

## Built with

- ASP.NET Core (.NET 10)
- EF Core + Npgsql
- SignalR
- xUnit + Testcontainers for integration tests

## Running locally

```bash
# Postgres must be up first (see repo root)
dotnet run --project src/ControlPlane.Api
```

Serves the REST API and SignalR hub on port `5000`. Migrations apply on startup. OpenAPI/Swagger is exposed in development, and the dashboard generates its typed client from that spec.

## Tests

```bash
dotnet test
```

Integration tests spin up a real Postgres in a container, so Docker must be running.
