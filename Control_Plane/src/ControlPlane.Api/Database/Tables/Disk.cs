using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControlPlane.Api.Database.Tables;

public class Disk
{
    public int Id { get; set; }
    public int MetricId { get; set; }
    public long Total { get; set; }
    public double UsedPercent { get; set; }
    public long Used { get; set; }
    public long Free { get; set; }

    public Metrics RecordHeader { get; set; }
}

public class DiskConfiguration : IEntityTypeConfiguration<Disk>
{
    public void Configure(EntityTypeBuilder<Disk> builder)
    {
        builder.ToTable("Disk");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.MetricId);
        builder.Property(x => x.Total);
        builder.Property(x => x.UsedPercent);
        builder.Property(x => x.Used);
        builder.Property(x => x.Free);

        builder.HasOne(x => x.RecordHeader)
            .WithOne(y => y.Disk)
            .HasForeignKey<Disk>(x => x.MetricId);
    }
}