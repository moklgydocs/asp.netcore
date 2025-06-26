using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using IdentityServer.Models;

namespace IdentityServer.Data
{
    /// <summary>
    /// 应用程序数据库上下文
    /// 集成ASP.NET Core Identity
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 配置ApplicationUser实体
            builder.Entity<ApplicationUser>(entity =>
            {
                // 扩展属性配置
                entity.Property(e => e.DisplayName).HasMaxLength(100);
                entity.Property(e => e.FirstName).HasMaxLength(50);
                entity.Property(e => e.LastName).HasMaxLength(50);
                entity.Property(e => e.AvatarUrl).HasMaxLength(500);
                entity.Property(e => e.Permissions).HasMaxLength(1000);
                entity.Property(e => e.Department).HasMaxLength(100);
                
                // 索引
                entity.HasIndex(e => e.Email).HasDatabaseName("IX_Users_Email");
                entity.HasIndex(e => e.IsActive).HasDatabaseName("IX_Users_IsActive");
                entity.HasIndex(e => e.CreatedAt).HasDatabaseName("IX_Users_CreatedAt");
            });

            // 自定义表名（可选）
            builder.Entity<ApplicationUser>().ToTable("Users");
        }
    }
}