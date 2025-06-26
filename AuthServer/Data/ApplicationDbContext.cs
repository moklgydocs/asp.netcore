using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AuthServer.Models;

namespace AuthServer.Data
{
    /// <summary>
    /// 应用程序数据库上下文
    /// 集成ASP.NET Core Identity和OpenIddict
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
                // 基本属性配置
                entity.Property(e => e.DisplayName).HasMaxLength(100);
                entity.Property(e => e.FirstName).HasMaxLength(50);
                entity.Property(e => e.LastName).HasMaxLength(50);
                entity.Property(e => e.AvatarUrl).HasMaxLength(500);
                entity.Property(e => e.Gender).HasMaxLength(10);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.City).HasMaxLength(100);
                entity.Property(e => e.Country).HasMaxLength(100);
                entity.Property(e => e.PostalCode).HasMaxLength(20);
                entity.Property(e => e.Permissions).HasMaxLength(2000);
                entity.Property(e => e.Department).HasMaxLength(100);
                entity.Property(e => e.JobTitle).HasMaxLength(100);
                entity.Property(e => e.EmployeeId).HasMaxLength(50);
                entity.Property(e => e.Notes).HasMaxLength(1000);

                // 索引配置
                entity.HasIndex(e => e.Email).HasDatabaseName("IX_Users_Email");
                entity.HasIndex(e => e.IsActive).HasDatabaseName("IX_Users_IsActive");
                entity.HasIndex(e => e.CreatedAt).HasDatabaseName("IX_Users_CreatedAt");
                entity.HasIndex(e => e.Department).HasDatabaseName("IX_Users_Department");
                entity.HasIndex(e => e.EmployeeId).HasDatabaseName("IX_Users_EmployeeId");
                entity.HasIndex(e => e.LastLoginAt).HasDatabaseName("IX_Users_LastLoginAt");

                // 默认值配置
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CanDelete).HasDefaultValue(true);
                entity.Property(e => e.LoginCount).HasDefaultValue(0);

                // 自引用关系（管理员关系）
                entity.HasOne<ApplicationUser>()
                      .WithMany()
                      .HasForeignKey(e => e.ManagerId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // 自定义表名（可选）
            builder.Entity<ApplicationUser>().ToTable("Users");
        }

        /// <summary>
        /// 保存更改时自动更新UpdatedAt字段
        /// </summary>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// 保存更改时自动更新UpdatedAt字段
        /// </summary>
        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        /// <summary>
        /// 更新时间戳
        /// </summary>
        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries<ApplicationUser>()
                .Where(e => e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}