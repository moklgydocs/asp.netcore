namespace MokPermissions.Web.HttpApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
             Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // ¹ØÁª Startup Àà
                    webBuilder.UseStartup<Startup>();
                }).Build().Run(); 
        }
    }
}
