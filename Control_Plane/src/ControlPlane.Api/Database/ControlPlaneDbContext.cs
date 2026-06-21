using System.Reflection;
using ControlPlane.Api.Database.Tables;
using Microsoft.EntityFrameworkCore;

namespace ControlPlane.Api.Database;

public class ControlPlaneDbContext : DbContext
{
    public ControlPlaneDbContext(DbContextOptions<ControlPlaneDbContext> options) : base(options)
    {
    }

    public DbSet<CPU> CPUs { get; set; }
    public DbSet<Disk> Disks { get; set; }
    public DbSet<Tables.Host> Hosts { get; set; }
    public DbSet<Memory> Memory { get; set; }
    public DbSet<Metrics> Metrics { get; set; } 
    public DbSet<Network> Networks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}