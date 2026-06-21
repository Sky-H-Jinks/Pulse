use axum::{Json, extract::State, http::StatusCode};
use sqlx::PgPool;
use crate::db::insert_payload;

use crate::payload::Payload;

pub async fn ingest_handler(
    State(pool): State<PgPool>,
    Json(payload): Json<Payload>,
) -> Result<StatusCode, (StatusCode, String)> {
    validate_payload(&payload).map_err(|e| (StatusCode::BAD_REQUEST, e))?;
    insert_payload(&pool, &payload).await.map_err(|e| {
        eprintln!("insert failed: {e}");
        (StatusCode::INTERNAL_SERVER_ERROR, e.to_string())
    })?;
    Ok(StatusCode::CREATED)
}


fn validate_payload(payload: &Payload) -> Result<(), String> {
    for reading in &payload.system_info {
        
        // CPU validation
        for cpu in &reading.cpu.cpu_data {
            validate_is_inbetween_with_id(cpu.cpu_id, "CPU".to_string(), &cpu.cpu_usage, &0.0, &100.0, true)?;
        }

        // Disk validation
        let disk_info = &reading.disk;

        validate_smaller_than(&disk_info.used, &disk_info.total)?;
        validate_is_inbetween(&disk_info.free, &0, &disk_info.total, true)?;
        validate_is_inbetween(&disk_info.used_percent, &0.0, &100.0, true)?;

        // Host validation

        let host_info = &reading.host;

        validate_string_not_empty(&"Host1".to_string(), &host_info.host_id)?;
        validate_string_not_empty(&"Host2".to_ascii_lowercase(), &host_info.hostname)?;

        // Memory

        let memory_info = &reading.memory;
        
        validate_smaller_than(&memory_info.used, &memory_info.total)?;
        validate_is_inbetween(&memory_info.free, &0, &memory_info.total, true)?;
        validate_is_inbetween(&memory_info.used_percent, &0.0, &100.0, true)?;

        // Network
        let network_info = &reading.network;

        for device in &network_info.network_devices {
            validate_string_not_empty(&"Network".to_string(), &device.name)?;
        }
    }

    Ok(())
}

fn validate_smaller_than<T: PartialOrd + std::fmt::Display>(x: &T, y: &T) -> Result<(), String> {
    if x > y {
        return Err("Is not valid".to_string());
    }
    Ok(())
}

fn validate_is_inbetween<T: PartialOrd + std::fmt::Display>(x: &T, smallest_value: &T, largest_value: &T, allow_largest_value: bool) -> Result<(), String> {

    let is_invalid = if allow_largest_value {
        !(smallest_value..=largest_value).contains(&x)
    } else {
        !(smallest_value..largest_value).contains(&x)
    };

    if is_invalid {
        return Err(format!("{} is not between {} and {}", x, smallest_value, largest_value));
    }

    Ok(())
}

fn validate_is_inbetween_with_id<T: PartialOrd + std::fmt::Display>(id: i64, source: String, x: &T, smallest_value: &T, largest_value: &T, allow_largest_value: bool) -> Result<(), String> {
    validate_is_inbetween(x, smallest_value, largest_value, allow_largest_value).map_err(|e| format!("Source {} : ID {} | {}", &source, &id, e))
}

fn validate_string_not_empty(source: &str, x: &str) -> Result<(), String> {
    if x.is_empty() {
        return Err(format!("({}) Field is empty '{}'", source, x));
    }

    Ok(())
}