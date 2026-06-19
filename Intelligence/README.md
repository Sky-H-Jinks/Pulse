# intelligence

The analysis service, written in Python on FastAPI. Watches metric streams for anomalies and generates plain-English incident summaries using an LLM. This is the layer that distinguishes Pulse from a plain status page.

## Why Python

Anomaly detection is Python's home territory — pandas/polars for windowed analysis, numpy for the statistics, scikit-learn available if the approach needs upgrading. The LLM side is Python-first too, via the Anthropic SDK. FastAPI keeps the service layer thin and typed.

## What it does

- A scheduled job that reads recent metrics, flags outliers, and writes them to the `anomalies` table (starting with rolling z-score / EWMA — explainable before fancy)
- `POST /summaries/incident/{id}` — gathers an incident's timeline and nearby anomalies, asks Claude for a structured summary (what happened, probable cause, suggested checks), and returns it for the control-plane to store

It annotates; it does not make alerting decisions — those belong to the control-plane. It does not own the schema.

## Built with

- [FastAPI](https://fastapi.tiangolo.com)
- [uv](https://github.com/astral-sh/uv) — dependency management
- [polars](https://pola.rs) — metric analysis
- [APScheduler](https://apscheduler.readthedocs.io) — the detection loop
- [anthropic](https://github.com/anthropics/anthropic-sdk-python) — LLM summaries

## Running locally

```bash
uv sync
uv run uvicorn app.main:app --reload --port 8000
```

Requires `DATABASE_URL` and `ANTHROPIC_API_KEY` in the environment.