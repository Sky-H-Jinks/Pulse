namespace ControlPlane.Api.Dtos.Hosts;

public record DiskDetailsDTO(
    double UsedPercent,
    long Free,
    long Used,
    long Total
);