using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
    /// 动态权限配置
    /// </summary>
    public class DynamicPermissionConfiguration : IEntityTypeConfiguration<DynamicPermission>
    {
        public void Configure(EntityTypeBuilder<DynamicPermission> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.DisplayName)
                .HasMaxLength(128);

            builder.Property(x => x.ParentName)
                .HasMaxLength(128);

            builder.Property(x => x.Description)
                .HasMaxLength(256);

            builder.Property(x => x.GroupName)
                .HasMaxLength(128);

            // 添加唯一索引
            builder.HasIndex(x => new { x.Name, x.TenantId })
                .IsUnique();
        }
    }
}
