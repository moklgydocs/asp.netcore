using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MokPermissions.Domain;

namespace MokPermissions.EntityframeworkCore
{
    /// <summary>
    /// 权限授权实体配置
    /// </summary>
    public class PermissionGrantConfiguration : IEntityTypeConfiguration<PermissionGrant>
    {
        public void Configure(EntityTypeBuilder<PermissionGrant> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.ProviderName)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(x => x.ProviderKey)
                .IsRequired()
                .HasMaxLength(64);

            // 创建联合唯一索引
            builder.HasIndex(x => new { x.Name, x.ProviderName, x.ProviderKey })
                .IsUnique();
        }
    }
}
