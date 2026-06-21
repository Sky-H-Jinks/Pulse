using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControlPlane.Api.Database.Tables;

public class Metrics
{
    public int Id { get; set; }
    public int HostId { get; set; }
    public CPU CPU { get; set; }
    public Memory Memory { get; set; }
    public Disk Disk { get; set; }
    public Network Network { get; set; }
    public Host Host { get; set; }
}

public class MetricsConfiguration : IEntityTypeConfiguration<Metrics>
{
    public void Configure(EntityTypeBuilder<Metrics> builder)
    {
        builder.ToTable("Metrics");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.HostId);

        builder.HasOne(x => x.Host)
            .WithMany(h => h.Metrics)
            .HasForeignKey(x => x.HostId);
    }
}