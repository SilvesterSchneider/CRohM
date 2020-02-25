using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace WebApi
{
    public class Program
    {
        /// <summary>
        /// Entry point for project
        /// </summary>
        /// <param name="args">arguments when starting from console e.g.</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// creates the webservice
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        // ****************************************** //
        // IMPORTANT: has to stay for testing purpose
        //public static IWebHostBuilder CreateWebHostBuilder(string[] args) => new WebHostBuilder()
        //    .ConfigureAppConfiguration((ctx, builder) =>
        //        {
        //        }
        //    )
        //    .UseStartup<Startup>()
        //    .ConfigureLogging((hostingContext, logging) => { });

        // ****************************************** //
    }
}