using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Id4sIdentityServer.Services;
using Id4sIdentityServer.Data;
using Id4sIdentityServer.Models;

var builder = WebApplication.CreateBuilder(args);

// 添加服务
builder.Services.AddControllersWithViews();

// 配置数据库上下文
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 使用内存数据库用于演示（生产环境请使用SQL Server等）
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("IdentityServerDb"));

// 配置ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // 密码策略配置
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    
    // 用户配置
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    
    // 登录配置
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// 配置Identity Server 4
var identityServerBuilder = builder.Services.AddIdentityServer(options =>
{
    // 配置Identity Server选项
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;
    
    // 发布者URI
    options.IssuerUri = "https://localhost:5001";
    
    // 用户交互选项
    options.UserInteraction.LoginUrl = "/Account/Login";
    options.UserInteraction.LogoutUrl = "/Account/Logout";
    options.UserInteraction.ErrorUrl = "/Home/Error";
})
.AddInMemoryIdentityResources(Config.IdentityResources)      // 身份资源
.AddInMemoryApiScopes(Config.ApiScopes)                      // API作用域
.AddInMemoryApiResources(Config.ApiResources)                // API资源
.AddInMemoryClients(Config.Clients)                          // 客户端配置
.AddAspNetIdentity<ApplicationUser>()                        // 集成ASP.NET Core Identity
.AddProfileService<ProfileService>();                        // 自定义用户档案服务

// 开发环境使用临时密钥（生产环境需要使用真实证书）
if (builder.Environment.IsDevelopment())
{
    identityServerBuilder.AddDeveloperSigningCredential();
}

// 注册自定义服务
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// 配置HTTP请求管道
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Identity Server中间件（必须在UseRouting之后，UseAuthorization之前）
app.UseIdentityServer();

app.UseAuthorization();

// 配置路由
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 初始化数据库
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    
    await SeedData.InitializeAsync(context, userManager, roleManager);
}

app.Run();