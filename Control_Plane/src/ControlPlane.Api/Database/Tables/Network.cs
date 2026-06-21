using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControlPlane.Api.Database.Tables;

public class Network
{
    public int Id { get; set; }
    public int MetricId { get; set; }
    public List<NetworkDetails> Details { get; set; } = [];
    public Metrics RecordHeader { get; set; }
}

public class NetworkConfiguration : IEntityTypeConfiguration<Network>
{
    public void Configure(EntityTypeBuilder<Network> builder)
    {
        builder.ToTable("Network");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.MetricId);

        builder.HasOne(x => x.RecordHeader)
            .WithOne(y => y.Network)
            .HasForeignKey<Network>(x => x.MetricId);
    }
}