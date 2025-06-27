using Microsoft.EntityFrameworkCore;
using ApiResource.Models;

namespace ApiResource.Data
{
    /// <summary>
    /// API数据库上下文
    /// </summary>
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 配置Product实体
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Price);
                entity.Property(e => e.CreatedAt);
                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.IsActive);
            });

            // 配置Customer实体
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.CreatedAt);
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // 配置Order实体
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TotalAmount);
                entity.Property(e => e.CreatedAt);
                
                entity.HasOne(e => e.Customer)
                      .WithMany(c => c.Orders)
                      .HasForeignKey(e => e.CustomerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.OrderNumber).IsUnique();
                entity.HasIndex(e => e.Status);
            });

            // 配置OrderItem实体
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UnitPrice);
                entity.Property(e => e.TotalPrice);

                entity.HasOne(e => e.Order)
                      .WithMany(o => o.Items)
                      .HasForeignKey(e => e.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                      .WithMany()
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }

    /// <summary>
    /// 种子数据初始化
    /// </summary>
    public static class SeedData
    {
        public static async Task InitializeAsync(ApiDbContext context)
        {
            await context.Database.EnsureCreatedAsync();

            if (context.Products.Any())
                return; // 数据已存在

            // 创建示例产品
            var products = new[]
            {
                new Product
                {
                    Name = "笔记本电脑",
                    Description = "高性能商务笔记本电脑",
                    Price = 8999.00m,
                    Category = "电子产品",
                    Stock = 50,
                    IsActive = true
                },
                new Product
                {
                    Name = "无线鼠标",
                    Description = "人体工学无线鼠标",
                    Price = 129.00m,
                    Category = "电脑配件",
                    Stock = 200,
                    IsActive = true
                },
                new Product
                {
                    Name = "机械键盘",
                    Description = "RGB背光机械键盘",
                    Price = 599.00m,
                    Category = "电脑配件",
                    Stock = 80,
                    IsActive = true
                },
                new Product
                {
                    Name = "显示器",
                    Description = "27寸4K显示器",
                    Price = 2999.00m,
                    Category = "电子产品",
                    Stock = 30,
                    IsActive = true
                },
                new Product
                {
                    Name = "办公椅",
                    Description = "人体工学办公椅",
                    Price = 1299.00m,
                    Category = "办公用品",
                    Stock = 25,
                    IsActive = true
                }
            };

            context.Products.AddRange(products);

            // 创建示例客户
            var customers = new[]
            {
                new Customer
                {
                    Name = "张三",
                    Email = "zhangsan@example.com",
                    Phone = "13800138000",
                    Address = "北京市朝阳区xxx街道xxx号"
                },
                new Customer
                {
                    Name = "李四",
                    Email = "lisi@example.com",
                    Phone = "13800138001",
                    Address = "上海市浦东新区xxx路xxx号"
                },
                new Customer
                {
                    Name = "王五",
                    Email = "wangwu@example.com",
                    Phone = "13800138002",
                    Address = "广州市天河区xxx大道xxx号"
                }
            };

            context.Customers.AddRange(customers);
            await context.SaveChangesAsync();

            // 创建示例订单
            var orders = new[]
            {
                new Order
                {
                    OrderNumber = "ORD202506260001",
                    CustomerId = customers[0].Id,
                    Status = OrderStatus.Completed,
                    TotalAmount = 9728.00m,
                    Items = new List<OrderItem>
                    {
                        new OrderItem { ProductId = products[0].Id, Quantity = 1, UnitPrice = 8999.00m, TotalPrice = 8999.00m },
                        new OrderItem { ProductId = products[1].Id, Quantity = 1, UnitPrice = 129.00m, TotalPrice = 129.00m },
                        new OrderItem { ProductId = products[2].Id, Quantity = 1, UnitPrice = 599.00m, TotalPrice = 599.00m }
                    }
                },
                new Order
                {
                    OrderNumber = "ORD202506260002",
                    CustomerId = customers[1].Id,
                    Status = OrderStatus.Processing,
                    TotalAmount = 2999.00m,
                    Items = new List<OrderItem>
                    {
                        new OrderItem { ProductId = products[3].Id, Quantity = 1, UnitPrice = 2999.00m, TotalPrice = 2999.00m }
                    }
                }
            };

            context.Orders.AddRange(orders);
            await context.SaveChangesAsync();
        }
    }
}