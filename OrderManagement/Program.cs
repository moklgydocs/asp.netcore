
using MediatR;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// 添加服务
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 添加数据库上下文
builder.Services.AddDbContext<OrderContext>(options =>
    options.UseInMemoryDatabase("OrderManagementDb"));

// 添加MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommand).Assembly);
});

// 添加应用层服务
builder.Services.AddApplicationServices();

// 添加基础设施层服务
builder.Services.AddInfrastructureServices();

var app = builder.Build();

// 配置HTTP管道
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// 确保数据库已创建
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<OrderContext>();
    context.Database.EnsureCreated();
}

app.Run();