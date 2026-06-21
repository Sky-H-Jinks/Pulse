using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControlPlane.Api.Database.Tables;

public class CPUDetails
{
    public int Id { get; set; }
    public int HeaderId { get; set; }
    public int CPUID { get; set; }
    public double CPUUsage { get; set; }

    public CPU? Header { get; set; }
}

public class CPUDetailsConfiguration : IEntityTypeConfiguration<CPUDetails>
{
    public void Configure(EntityTypeBuilder<CPUDetails> builder)
    {
        builder.ToTable("CPU_Details");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.HeaderId);

        builder.Property(x => x.CPUID);

        builder.Property(x => x.CPUUsage);

        builder.HasOne(x => x.Header)
            .WithMany(h => h.Details)
            .HasForeignKey(x => x.HeaderId);
    }
}