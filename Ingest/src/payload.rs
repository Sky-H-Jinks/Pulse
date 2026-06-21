use serde::Deserialize;

#[derive(Deserialize, Debug)]
#[allow(dead_code)]
pub struct Payload {
    pub system_info: Vec<SystemInfo>,
}

#[derive(Deserialize, Debug)]
#[allow(dead_code)]
pub struct SystemInfo {
    pub timestamp: String,
    pub cpu: CpuPayload,
    pub memory: MemoryPayload,
    pub disk: DiskPayload,
    pub network: NetworkPayload,
    pub host: HostPayload,
}

#[derive(Deserialize, Debug)]
#[allow(dead_code)]
pub struct CpuPayload {
    pub cpu_data: Vec<CpuInfoPayload>,

}

#[derive(Deserialize, Debug)]
#[allow(dead_code)]
pub struct CpuInfoPayload {
    pub cpu_id: i64,
    pub cpu_usage: f64,
}

#[derive(Deserialize, Debug)]
pub struct MemoryPayload {
    pub total: u64,
    pub used: u64,
    pub available: u64,
    pub used_percent: f64,
    pub free: u64,
}

#[derive(Deserialize, Debug)]
#[allow(dead_code)]
pub struct DiskPayload {
    pub total: u64,
    pub used: u64,
    pub free: u64,
    pub used_percent: f64,
}

#[derive(Deserialize, Debug)]
#[allow(dead_code)]
pub struct NetworkPayload {
    pub network_devices: Vec<NetworkDevicePayload>,
}

#[derive(Deserialize, Debug)]
#[allow(dead_code)]
pub struct NetworkDevicePayload {
    pub name: String,
    pub bytes_sent: u64,
    pub bytes_recv: u64,
    pub packets_sent: u64,
    pub packets_recv: u64,
}

#[derive(Deserialize, Debug)]
#[allow(dead_code)]
pub struct HostPayload {
    pub hostname: String,
    pub os: String,
    pub platform: String,
    pub platform_family: String,
    pub platform_version: String,
    pub kernel_version: String,
    pub host_id: String,
    pub boot_time: u64,
    pub uptime: u64,
}