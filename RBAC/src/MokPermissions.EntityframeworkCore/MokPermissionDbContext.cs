using Microsoft.EntityFrameworkCore;
using MokPermissions.Domain.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.EntityframeworkCore
{
    /// <summary>
    /// 权限管理数据库上下文
    /// </summary>
    public class MokPermissionDbContext : DbContext
    {
        public DbSet<PermissionGrant> PermissionGrants { get; set; }

        public DbSet<DynamicPermission> DynamicPermissions { get; set; }

        public MokPermissionDbContext(DbContextOptions<MokPermissionDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new PermissionGrantConfiguration());
            modelBuilder.ApplyConfiguration(new DynamicPermissionConfiguration());
        }
    }
}
