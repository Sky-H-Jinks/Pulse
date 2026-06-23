namespace ControlPlane.Api.Dtos.Hosts;

public record MemoryDetailsDTO(
    long Total,
    long Used,
    long Free,
    double UsagePercentage
);