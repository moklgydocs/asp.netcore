using Microsoft.EntityFrameworkCore;

namespace BasicAuth.Models
{
    /// <summary>
    /// 应用程序数据库上下文
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 配置User实体
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Role).IsRequired().HasMaxLength(20);
            });
        }
    }

    /// <summary>
    /// 数据库种子数据
    /// </summary>
    public static class SeedData
    {
        public static async Task Initialize(AppDbContext context)
        {
            // 确保数据库已创建
            await context.Database.EnsureCreatedAsync();

            // 如果已有数据，则不执行种子数据初始化
            if (context.Users.Any())
                return;

            var users = new[]
            {
                new User
                {
                    Username = "admin",
                    Email = "admin@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Role = "Admin",
                    Age = 30,
                    Permissions = "user.manage,system.admin,data.read,data.write"
                },
                new User
                {
                    Username = "manager",
                    Email = "manager@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("manager123"),
                    Role = "Manager",
                    Age = 28,
                    Permissions = "user.manage,data.read,data.write"
                },
                new User
                {
                    Username = "user",
                    Email = "user@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"),
                    Role = "User",
                    Age = 25,
                    Permissions = "data.read"
                },
                new User
                {
                    Username = "younguser",
                    Email = "young@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("young123"),
                    Role = "User",
                    Age = 16, // 用于测试年龄限制策略
                    Permissions = "data.read"
                }
            };

            context.Users.AddRange(users);
            await context.SaveChangesAsync();
        }
    }
}