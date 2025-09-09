using GatewayCenter;
using Microsoft.EntityFrameworkCore;

public class YarpConfigDbContext : DbContext
{
    public YarpConfigDbContext(DbContextOptions<YarpConfigDbContext> options) : base(options) { }

    public DbSet<YarpRoute> Routes { get; set; }
    public DbSet<YarpCluster> Clusters { get; set; }
    public DbSet<YarpDestination> Destinations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 配置主键和索引
        modelBuilder.Entity<YarpRoute>().HasKey(r => r.Id);
        modelBuilder.Entity<YarpRoute>().HasIndex(r => r.RouteId).IsUnique();

        modelBuilder.Entity<YarpCluster>().HasKey(c => c.Id);
        modelBuilder.Entity<YarpCluster>().HasIndex(c => c.ClusterId).IsUnique();

        modelBuilder.Entity<YarpDestination>().HasKey(d => d.Id);
        modelBuilder.Entity<YarpDestination>().HasIndex(d => new { d.ClusterId, d.DestinationId }).IsUnique();
    }
}