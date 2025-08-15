using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DDD.ECommerce.Domain.Catalog;
using DDDCore.Domain;
using Microsoft.EntityFrameworkCore;

namespace DDD.ECommerce.Infrastructure.Data
{
    /// <summary>
    /// 电子商务应用的数据库上下文
    /// </summary>
    public class ECommerceDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// 配置实体映射和关系
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 产品实体映射
            modelBuilder.Entity<Product>(entity =>
            {
                //entity.ToTable("Products");
                //entity.HasKey(e => e.Id);

                //// 乐观并发控制
                //entity.Property(e => e.Version).IsRowVersion();

                //// 复杂类型映射
                //entity.OwnsOne(e => e.Name, name =>
                //{
                //    name.Property(p => p.Value).HasColumnName("Name").IsRequired().HasMaxLength(100);
                //});

                //entity.OwnsOne(e => e.Description, description =>
                //{
                //    description.Property(p => p.Value).HasColumnName("Description").HasMaxLength(2000);
                //});

                //entity.OwnsOne(e => e.Price, price =>
                //{
                //    price.Property(p => p.Amount).HasColumnName("Price").HasColumnType("decimal(18,2)").IsRequired();
                //    price.Property(p => p.Currency).HasColumnName("Currency").IsRequired().HasMaxLength(3);
                //});

                //entity.OwnsOne(e => e.Category, category =>
                //{
                //    category.Property(p => p.Type).HasColumnName("CategoryType").IsRequired();
                //    category.Property(p => p.SubCategory).HasColumnName("SubCategory").HasMaxLength(50);
                //});

                //// 实体关系
                //entity.HasMany(p => p._images)
                //      .WithOne()
                //      .HasForeignKey("ProductId")
                //      .OnDelete(DeleteBehavior.Cascade);
            });

            // 产品图片实体映射
            modelBuilder.Entity<ProductImage>(entity =>
            {
                //entity.ToTable("ProductImages");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Url).IsRequired().HasMaxLength(500);
                entity.Property(e => e.ContentType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);

                // 添加产品外键
                entity.Property<Guid>("ProductId").IsRequired();
            });
        }

        /// <summary>
        /// 获取所有待处理的领域事件
        /// </summary>
        public async Task<DomainEvent[]> GetDomainEventsAsync()
        {
            // 获取所有已跟踪且具有领域事件的聚合根
            var aggregateRoots = ChangeTracker.Entries<AggregateRoot<Guid>>()
                .Where(x => x.Entity.DomainEvents.Any())
                .Select(x => x.Entity)
                .ToArray();

            // 收集所有领域事件
            var domainEvents = aggregateRoots
                .SelectMany(x => x.DomainEvents)
                .ToArray();

            return domainEvents;
        }

        /// <summary>
        /// 清除所有已处理的领域事件
        /// </summary>
        public void ClearDomainEvents()
        {
            var aggregateRoots = ChangeTracker.Entries<AggregateRoot<Guid>>()
                .Where(x => x.Entity.DomainEvents.Any())
                .Select(x => x.Entity)
                .ToArray();

            foreach (var aggregate in aggregateRoots)
            {
                aggregate.ClearDomainEvents();
            }
        }

        /// <summary>
        /// 重写保存更改方法，添加审计信息等
        /// </summary>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            // 这里可以添加审计字段自动填充、软删除处理等
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}