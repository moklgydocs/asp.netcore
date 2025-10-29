using EfCore.Application.Contracts;
using EfCore.Applications;
using EfCore.Pgsql;
using Mok.SqlFactory;
using Mok.SqlFactory.Factory;

namespace EfCore.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDefaultDbContext<PgDbContext>();
            // Add services to the container.
            builder.Services.AddScoped<IEFCorePractiseAppServices, EFCorePractiseAppServices>();
            builder.Services.AddRazorPages();
            builder.Services.AddControllers();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.MapGet("/", (HttpContext context) =>
            {
                context.Response.WriteAsync("helloworld");
            });
            app.UseAuthorization();
            app.MapControllers();

            app.MapRazorPages();

            app.Run();
        }
    }
}
