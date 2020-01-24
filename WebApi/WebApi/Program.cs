using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        // ****************************************** //
        // IMPORTANT: has to stay for Testing
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) => new WebHostBuilder()
            .ConfigureAppConfiguration((ctx, builder) =>
                {
                }
            )
            .UseStartup<Startup>()
            .ConfigureLogging((hostingContext, logging) => { });

        // ****************************************** //
    }
}