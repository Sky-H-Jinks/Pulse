using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControlPlane.Api.Database.Tables;

public class NetworkDetails
{
    public int Id { get; set; }
    public int NetworkId { get; set; }
    public string NetworkName { get; set; }
    public long BytesSent { get; set; }
    public long BytesReceived { get; set; }
    public long PacketsSent { get; set; }
    public long PacketsReceived { get; set; }

    public Network Header { get; set; }
}

public class NetworkDetailsConfiguration : IEntityTypeConfiguration<NetworkDetails>
{
    public void Configure(EntityTypeBuilder<NetworkDetails> builder)
    {
        builder.ToTable("Network_Details");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.NetworkName)
            .IsRequired()
            .HasMaxLength(1024);

        builder.Property(x => x.NetworkId);
        builder.Property(x => x.BytesSent);
        builder.Property(x => x.BytesReceived);
        builder.Property(x => x.PacketsSent);
        builder.Property(x => x.PacketsReceived);

        builder.HasOne(x => x.Header)
            .WithMany(y => y.Details)
            .HasForeignKey(x => x.NetworkId);
    }
}