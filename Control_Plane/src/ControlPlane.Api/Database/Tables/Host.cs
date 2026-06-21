using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControlPlane.Api.Database.Tables;

public class Host
{
    public int Id { get; set; }
    public string Hostname { get; set; }
    public string OS { get; set; }
    public string Platform { get; set; }
    public string PlatformFamily { get; set; }
    public string PlatformVersion { get; set; }
    public string Kernal { get; set; }
    public string HostID { get; set; }
    public string BootTime { get; set; }
    public string Uptime { get; set; }

    public List<Metrics> Metrics { get; set; } = [];
}

public class HostConfiguration : IEntityTypeConfiguration<Host>
{
    public void Configure(EntityTypeBuilder<Host> builder)
    {
        builder.ToTable("Host");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Hostname)
            .IsRequired()
            .HasMaxLength(1024);

        builder.Property(x => x.OS)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.Platform)
            .IsRequired(false)
            .HasMaxLength(1024);

        builder.Property(x => x.PlatformFamily)
            .IsRequired(false)
            .HasMaxLength(1024);

        builder.Property(x => x.PlatformVersion)
            .IsRequired(false)
            .HasMaxLength(128);

        builder.Property(x => x.Kernal)
            .IsRequired()
            .HasMaxLength(1024);

        builder.Property(x => x.HostID)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.BootTime)
            .IsRequired()
            .HasMaxLength(1024);

        builder.Property(x => x.Uptime)
            .IsRequired()
            .HasMaxLength(1024);
    }
}