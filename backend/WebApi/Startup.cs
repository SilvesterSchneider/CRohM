using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer;
using ServiceLayer;
using WebApi.Helper;

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

            services.AddAutoMapper(typeof(Startup));

            //TODO: configure swagger
            services.AddSwaggerDocument();

            services.AddIdentity<User, Role>(options =>
                {
                    //TODO: check with Product owner if he still wants to login with admin admin -> therefor i need this password settings
                    //// Password settings.
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 5;
                    options.Password.RequiredUniqueChars = 0;

                    options.User.RequireUniqueEmail = true;
                    options.SignIn.RequireConfirmedAccount = false;
                })
                .AddSignInManager<SignInService>()
                .AddUserManager<UserService>()
                .AddEntityFrameworkStores<CrmContext>();

            services.AddDbContext<CrmContext>(config =>
                {
                    config.UseSqlServer(Configuration.GetConnectionString("LocalDb"));
                });

            //TODO: configure JWT validation

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "wwwroot";
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserService userService)
        {
            ApplicationDbInitializer.SeedUsers(userService);
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "wwwroot";
            });
        }

        private void AddDependencyInjection(IServiceCollection services)
        {
            //###########################Services#######################################

            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IEducationalOpportunityService, EducationalOpportunityService>();

            //###########################Repositories#######################################

            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IEducationalOpportunityRepository, EducationalOpportunityRepository>();
        }
    }
}