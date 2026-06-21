using System.Numerics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControlPlane.Api.Database.Tables;

public class Memory
{
    public int Id { get; set; }
    public int MetricId { get; set; }
    public long Total { get; set; }
    public long Used { get; set; }
    public long Available { get; set; }
    public double UsedPercent { get; set; }
    public long Free { get; set; }

    public Metrics RecordHeader { get; set; }
}

public class MemoryConfiguration : IEntityTypeConfiguration<Memory>
{
    public void Configure(EntityTypeBuilder<Memory> builder)
    {
        builder.ToTable("Memory");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.MetricId);
        builder.Property(x => x.Total);
        builder.Property(x => x.Used);
        builder.Property(x => x.Available);
        builder.Property(x => x.UsedPercent);
        builder.Property(x => x.Free);

        builder.HasOne(x => x.RecordHeader)
            .WithOne(y => y.Memory)
            .HasForeignKey<Memory>(x => x.MetricId);
    }
}