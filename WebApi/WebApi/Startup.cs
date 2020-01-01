using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelLayer;
using RepositoryLayer;
using ServiceLayer;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            AddDependencyInjection(services);
            StartupExtension.ConfigureServices(services);

            services.AddSwaggerDocument();

            //TODO configure asp net identity
            //TODO configure jwt
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //TODO: implement middleware for exceptions
            //TODO: catch exception - DbUpdateConcurrencyException
            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void AddDependencyInjection(IServiceCollection services)
        {
            services.AddDbContext<CrmContext>(options =>
                options.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"]));

            //###########################Services#######################################

            services.AddScoped<IAddressService, AddressService>();

            //###########################Repositories#######################################

            services.AddScoped<IAddressRepository, AddressRepository>();
        }
    }
}