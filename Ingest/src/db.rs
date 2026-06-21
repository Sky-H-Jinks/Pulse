use sqlx::{PgPool, Postgres, Transaction};
use crate::payload::{CpuPayload, DiskPayload, HostPayload, MemoryPayload, Payload, SystemInfo, NetworkPayload};

pub async fn insert_payload(pool: &PgPool, payload: &Payload) -> Result<(), sqlx::Error> {
    for entry in &payload.system_info {
        insert_entry(pool, &entry).await?;
    }
    Ok(())
}

async fn insert_entry(pool: &PgPool, payload: &SystemInfo) -> Result<(), sqlx::Error> {
    let mut tx = pool.begin().await?;
    
    let host_id  = upsert_host(&mut tx, &payload.host).await?;
    let metric_id = insert_metric(&mut tx, host_id).await?;
    insert_cpu(&mut tx, metric_id, &payload.cpu).await?;
    insert_disk(&mut tx, metric_id, &payload.disk).await?;
    insert_memory(&mut tx, metric_id, &payload.memory).await?;
    insert_network(&mut tx, metric_id, &payload.network).await?;

    tx.commit().await?;
    Ok(())
}

async fn upsert_host(tx: &mut Transaction<'_, Postgres>, host: &HostPayload) -> Result<i32, sqlx::Error> {
    let existing: Option<(i32,)> = sqlx::query_as(r#"SELECT "Id" FROM "Host" WHERE "HostID" = $1"#)
        .bind(&host.host_id)
        .fetch_optional(tx.as_mut())
        .await?;

    match existing {
        Some(row) => Ok(row.0),
        None => {
            let row: (i32,) = sqlx::query_as(r#"INSERT INTO "Host" ("Hostname", "OS", "Platform", "PlatformFamily", "PlatformVersion", "Kernal", "HostID", "BootTime", "Uptime") VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9) RETURNING "Id""#)
                .bind(&host.hostname)
                .bind(&host.os)
                .bind(&host.platform)
                .bind(&host.platform_family)
                .bind(&host.platform_version)
                .bind(&host.kernel_version)
                .bind(&host.host_id)
                .bind(host.boot_time as i64)
                .bind(host.uptime as i64)
                .fetch_one(tx.as_mut())
                .await?;
            Ok(row.0)
        }
    }
}

async fn insert_metric(tx: &mut Transaction<'_, Postgres>, host_id: i32) -> Result<i32, sqlx::Error> {
    let row: (i32,) = sqlx::query_as(r#"INSERT INTO "Metrics" ("HostId") VALUES ($1) RETURNING "Id""#)
        .bind(host_id)
        .fetch_one(tx.as_mut())
        .await?;

    Ok(row.0)
}

async fn insert_cpu(tx: &mut Transaction<'_, Postgres>, metric_id: i32, cpu_payload: &CpuPayload) -> Result<(), sqlx::Error> {
    let header_id: (i32, ) = sqlx::query_as(r#"INSERT INTO "CPU_Header" ("MetricId") VALUES ($1) RETURNING "Id""#)
        .bind(metric_id)
        .fetch_one(tx.as_mut())
        .await?;

    for cpu in &cpu_payload.cpu_data {
        sqlx::query(r#"INSERT INTO "CPU_Details" ("HeaderId", "CPUID", "CPUUsage") VALUES ($1, $2, $3)"#)
            .bind(header_id.0)
            .bind(cpu.cpu_id as i32)
            .bind(cpu.cpu_usage)
            .execute(tx.as_mut())
            .await?;
    }

    Ok(())
}

async fn insert_disk(tx: &mut Transaction<'_, Postgres>, metric_id: i32, disk_payload: &DiskPayload) -> Result<(), sqlx::Error> {
    sqlx::query(r#"INSERT INTO "Disk" ("MetricId", "Total", "UsedPercent", "Used", "Free") VALUES ($1, $2, $3, $4, $5)"#)
        .bind(metric_id)
        .bind(disk_payload.total as i64)
        .bind(disk_payload.used_percent)
        .bind(disk_payload.used as i64)
        .bind(disk_payload.free as i64)
        .execute(tx.as_mut())
        .await?;

    Ok(())
}

async fn insert_memory(tx: &mut Transaction<'_, Postgres>, metric_id: i32, memory_payload: &MemoryPayload) -> Result<(), sqlx::Error> {
    sqlx::query(r#"INSERT INTO "Memory" ("MetricId", "Total", "Used", "Available", "UsedPercent", "Free") VALUES ($1, $2, $3, $4, $5, $6)"#)
        .bind(metric_id)
        .bind(memory_payload.total as i64)
        .bind(memory_payload.used as i64)
        .bind(memory_payload.available as i64)
        .bind(memory_payload.used_percent)
        .bind(memory_payload.free as i64)
        .execute(tx.as_mut())
        .await?;

    Ok(())
}

async fn insert_network(tx: &mut Transaction<'_, Postgres>, metric_id: i32, network_payload: &NetworkPayload) -> Result<(), sqlx::Error> {
    let headerid: (i32,) = sqlx::query_as(r#"INSERT INTO "Network" ("MetricId") VALUES ($1) RETURNING "Id""#)
        .bind(metric_id)
        .fetch_one(tx.as_mut())
        .await?;

    for device in &network_payload.network_devices {
        sqlx::query(r#"INSERT INTO "Network_Details" ("NetworkId", "NetworkName", "BytesSent", "BytesReceived", "PacketsSent", "PacketsReceived") VALUES ($1, $2, $3, $4, $5, $6)"#)
            .bind(headerid.0)
            .bind(&device.name)
            .bind(device.bytes_sent as i64)
            .bind(device.bytes_recv as i64)
            .bind(device.packets_sent as i64)
            .bind(device.packets_recv as i64)
            .execute(tx.as_mut())
            .await?;
    }

    Ok(())
}