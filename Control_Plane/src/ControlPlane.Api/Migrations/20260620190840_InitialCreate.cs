using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ControlPlane.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Host",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Hostname = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    OS = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Platform = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    PlatformFamily = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    PlatformVersion = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Kernal = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    HostID = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    BootTime = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    Uptime = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Host", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Metrics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HostId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metrics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Metrics_Host_HostId",
                        column: x => x.HostId,
                        principalTable: "Host",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CPU_Header",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MetricId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CPU_Header", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CPU_Header_Metrics_MetricId",
                        column: x => x.MetricId,
                        principalTable: "Metrics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Disk",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MetricId = table.Column<int>(type: "integer", nullable: false),
                    Total = table.Column<int>(type: "integer", nullable: false),
                    UsedPercent = table.Column<double>(type: "double precision", nullable: false),
                    Used = table.Column<int>(type: "integer", nullable: false),
                    Free = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disk", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Disk_Metrics_MetricId",
                        column: x => x.MetricId,
                        principalTable: "Metrics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Memory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MetricId = table.Column<int>(type: "integer", nullable: false),
                    Total = table.Column<double>(type: "double precision", nullable: false),
                    Used = table.Column<double>(type: "double precision", nullable: false),
                    Available = table.Column<int>(type: "integer", nullable: false),
                    UsedPercent = table.Column<double>(type: "double precision", nullable: false),
                    Free = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Memory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Memory_Metrics_MetricId",
                        column: x => x.MetricId,
                        principalTable: "Metrics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Network",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MetricId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Network", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Network_Metrics_MetricId",
                        column: x => x.MetricId,
                        principalTable: "Metrics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CPU_Details",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HeaderId = table.Column<int>(type: "integer", nullable: false),
                    CPUID = table.Column<int>(type: "integer", nullable: false),
                    CPUUsage = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CPU_Details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CPU_Details_CPU_Header_HeaderId",
                        column: x => x.HeaderId,
                        principalTable: "CPU_Header",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Network_Details",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NetworkId = table.Column<int>(type: "integer", nullable: false),
                    BytesSent = table.Column<long>(type: "bigint", nullable: false),
                    BytesReceived = table.Column<long>(type: "bigint", nullable: false),
                    PacketsSent = table.Column<long>(type: "bigint", nullable: false),
                    PacketsReceived = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Network_Details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Network_Details_Network_NetworkId",
                        column: x => x.NetworkId,
                        principalTable: "Network",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CPU_Details_HeaderId",
                table: "CPU_Details",
                column: "HeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_CPU_Header_MetricId",
                table: "CPU_Header",
                column: "MetricId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Disk_MetricId",
                table: "Disk",
                column: "MetricId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Memory_MetricId",
                table: "Memory",
                column: "MetricId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Metrics_HostId",
                table: "Metrics",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_Network_MetricId",
                table: "Network",
                column: "MetricId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Network_Details_NetworkId",
                table: "Network_Details",
                column: "NetworkId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CPU_Details");

            migrationBuilder.DropTable(
                name: "Disk");

            migrationBuilder.DropTable(
                name: "Memory");

            migrationBuilder.DropTable(
                name: "Network_Details");

            migrationBuilder.DropTable(
                name: "CPU_Header");

            migrationBuilder.DropTable(
                name: "Network");

            migrationBuilder.DropTable(
                name: "Metrics");

            migrationBuilder.DropTable(
                name: "Host");
        }
    }
}
