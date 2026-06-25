using ControlPlane.Api.Database;
using ControlPlane.Api.Dtos.Hosts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControlPlane.Api.Endpoints.Hosts;

[ApiController]
[Route("api/[Controller]")]
public class HostsController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Database.Tables.Host>>> GetHosts([FromServices] ControlPlaneDbContext dbContext)
    {
        var hosts = await dbContext.Hosts.AsNoTracking().ToListAsync();
        return Ok(hosts);
    }

    [HttpGet("{id}/metrics")]
    public async Task<ActionResult<IEnumerable<MetricDTO>>> GetHostMetrics(int id, [FromServices] ControlPlaneDbContext dbContext, DateTime? from = null, DateTime? to = null)
    {
        var host = await dbContext.Hosts.FindAsync(id);
        if (host == null)
        {
            return NotFound();
        }

        var metrics = await dbContext.Metrics
            .Where(m => m.HostId == id)
            .Where(m => from == null || m.Timestamp >= from)
            .Where(m => to == null || m.Timestamp <= to)
            .AsNoTracking()
            .OrderBy(m => m.Timestamp)
            .Select(m => new MetricDTO(
                m.Id,
                m.HostId,
                m.Timestamp,
                new CPUDetailsDTO(m.CPU.Details.Select(c => new CPUDetailInfoDTO(c.Id, c.CPUUsage)).ToList()),
                new MemoryDetailsDTO(m.Memory.Total, m.Memory.Used, m.Memory.Free, m.Memory.UsedPercent),
                new DiskDetailsDTO(m.Disk.UsedPercent, m.Disk.Free, m.Disk.Used, m.Disk.Total),
                new NetworkDetailsDTO(m.Network.Details.Select(n => new NetworkDetailsInfoDTO(n.NetworkName, n.BytesSent, n.BytesReceived, n.PacketsSent, n.PacketsReceived)).ToList())
            ))
        .ToListAsync();

        return Ok(metrics);
    }
}