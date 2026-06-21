mod payload;
mod db;
mod routes;

use axum::{Router, routing::{get, post}};
use sqlx::postgres::PgPoolOptions;
use crate::routes::ingest_handler;

#[tokio::main]
async fn main() -> Result<(), Box<dyn std::error::Error>> {
    let url = std::env::var("DATABASE_URL")?;
    let pool = PgPoolOptions::new().max_connections(5).connect(&url).await?;
    println!("db connected");

    let app = Router::new()
        .route("/api/v1/ingest", post(ingest_handler))
        .route("/healthz", get(|| async { "ok" }))
        .with_state(pool);

    let listener = tokio::net::TcpListener::bind("0.0.0.0:8080").await?;
    
    println!("listening on: localhost:8080");
    axum::serve(listener, app).await?;

    Ok(())
}