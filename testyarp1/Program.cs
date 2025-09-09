namespace testyarp1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddHealthChecks();
            var app = builder.Build();
            // 开发环境移除 HTTPS 重定向
            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseAuthorization();

            // 配置健康检查端点
            app.MapHealthChecks("/health");
            // 添加根路径处理
            app.MapGet("/", () => new { Service = "testyarp1", Status = "Running", Port = 5085 });

            app.MapControllers();

            app.Run();
        }
    }
}
