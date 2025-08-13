using Microsoft.EntityFrameworkCore;
using MokPermissions.Domain;
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
    public class PermissionManagementDbContext : DbContext
    {
        public DbSet<PermissionGrant> PermissionGrants { get; set; }

        public PermissionManagementDbContext(DbContextOptions<PermissionManagementDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new PermissionGrantConfiguration());
        }
    }
}
