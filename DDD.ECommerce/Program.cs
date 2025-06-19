using DDD.ECommerce.Application.DTOs;
using DDD.ECommerce.Application.EventHandlers;
using DDD.ECommerce.Application.Interfaces;
using DDD.ECommerce.Application.Services;
using DDD.ECommerce.Domain.Catalog;
using DDD.ECommerce.Domain.Catalog.Events;
using DDD.ECommerce.Infrastructure.Data;
using DDD.ECommerce.Infrastructure.Repositories;
using DDDCore.Domain;
using DDDCore.Infrastructure;
using Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 添加服务到容器
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// 配置Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ECommerce.WebAPI", Version = "v1" });
});

// 数据库配置
builder.Services.AddDbContext<ECommerceDbContext>(options =>
{
    // 使用内存数据库作为示例，实际应用中应使用持久存储
    options.UseInMemoryDatabase("ECommerceDb");
});

// 注册基础设施服务
builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
builder.Services.AddScoped<IUnitOfWork, ECommerceUnitOfWork>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// 注册应用服务
builder.Services.AddScoped<IProductService, ProductService>();

// 注册领域事件处理器
builder.Services.AddScoped<IDomainEventHandler<ProductCreatedEvent>, ProductCreatedEventHandler>();
builder.Services.AddScoped<IDomainEventHandler<ProductPriceChangedEvent>, ProductPriceChangedEventHandler>();
builder.Services.AddScoped<IDomainEventHandler<ProductStockChangedEvent>, ProductStockChangedEventHandler>();

// 添加跨域支持
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// 配置HTTP请求管道
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // 初始化测试数据
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            SeedData(services);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();

// 测试数据填充
void SeedData(IServiceProvider services)
{
    var productService = services.GetRequiredService<IProductService>();
    
    // 添加一些测试产品
    var tasks = new[]
    {
        productService.CreateProductAsync(new  CreateProductDto
        {
            Name = "笔记本电脑",
            Description = "高性能笔记本电脑，适合工作和游戏",
            Price = 5999.99m,
            Currency = "CNY",
            StockQuantity = 50,
            Category = CategoryType.Electronics,
            SubCategory = "Laptops"
        }),
        
        productService.CreateProductAsync(new CreateProductDto
        {
            Name = "智能手机",
            Description = "最新款智能手机，拥有强大的相机功能",
            Price = 3999.99m,
            Currency = "CNY",
            StockQuantity = 100,
            Category = CategoryType.Electronics,
            SubCategory = "Smartphones"
        }),
        
        productService.CreateProductAsync(new   CreateProductDto
        {
            Name = "运动鞋",
            Description = "舒适的运动鞋，适合长时间穿着",
            Price = 499.99m,
            Currency = "CNY",
            StockQuantity = 200,
            Category = CategoryType.Sports,
            SubCategory = "Footwear"
        }),
        
        productService.CreateProductAsync(new CreateProductDto
        {
            Name = "休闲T恤",
            Description = "100%纯棉材质，时尚舒适",
            Price = 129.99m,
            Currency = "CNY",
            StockQuantity = 500,
            Category = CategoryType.Clothing,
            SubCategory = "T-Shirts"
        }),
        
        productService.CreateProductAsync(new CreateProductDto
        {
            Name = "编程书籍",
            Description = "深入解析领域驱动设计原理与实践",
            Price = 89.99m,
            Currency = "CNY",
            StockQuantity = 30,
            Category = CategoryType.Books,
            SubCategory = "Programming"
        })
    };
    
    Task.WhenAll(tasks).GetAwaiter().GetResult();
}