using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using RepositoryLayer;
using ServiceLayer;

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
            // TODO delete
            PdfGenerator.generatePdf();
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
    }
}