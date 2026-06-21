using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControlPlane.Api.Database.Tables;

public class CPU
{
    public int Id { get; set; }
    public int MetricId { get; set; }
    public List<CPUDetails> Details { get; set; } = [];

    public Metrics RecordHeader { get; set; }
}

public class CPUConfiguration : IEntityTypeConfiguration<CPU>
{
    public void Configure(EntityTypeBuilder<CPU> builder)
    {
        builder.ToTable("CPU_Header");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(x => x.MetricId);

        builder.HasOne(x => x.RecordHeader)
            .WithOne(y => y.CPU)
            .HasForeignKey<CPU>(x => x.MetricId);
    }
}