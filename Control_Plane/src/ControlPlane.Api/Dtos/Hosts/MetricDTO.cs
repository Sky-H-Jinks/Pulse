namespace ControlPlane.Api.Dtos.Hosts;

public record MetricDTO(
    int Id,
    int HostId,
    DateTime Timestamp,
    CPUDetailsDTO CPU,
    MemoryDetailsDTO Memory,
    DiskDetailsDTO Disk,
    NetworkDetailsDTO Network
);