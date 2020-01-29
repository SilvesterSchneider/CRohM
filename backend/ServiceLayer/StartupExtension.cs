using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceLayer
{
    public static class StartupExtension
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(StartupExtension));
        }
    }
}