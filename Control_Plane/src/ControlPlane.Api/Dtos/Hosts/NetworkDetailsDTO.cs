namespace ControlPlane.Api.Dtos.Hosts;

public record NetworkDetailsDTO(
    IReadOnlyList<NetworkDetailsInfoDTO> Details
);

public record NetworkDetailsInfoDTO(
    string Name,
    long BytesSent,
    long BytesReceived,
    long PacketsSent,
    long PacketsReceived
);