namespace ControlPlane.Api.Dtos.Hosts;

public record CPUDetailsDTO(
    IReadOnlyList<CPUDetailInfoDTO> Details
);

public record CPUDetailInfoDTO (
    int Id,
    double Usage
);