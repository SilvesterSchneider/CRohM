using System;
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
                    //// Password settings.
                    //options.Password.RequireDigit = true;
                    //options.Password.RequireLowercase = true;
                    //options.Password.RequireNonAlphanumeric = true;
                    //options.Password.RequireUppercase = true;
                    //options.Password.RequiredLength = 6;
                    //options.Password.RequiredUniqueChars = 1;

                    options.User.RequireUniqueEmail = true;
                    options.SignIn.RequireConfirmedAccount = false;
                }).AddSignInManager<SignInService>()
                .AddUserManager<UserService>()
                .AddEntityFrameworkStores<CrmContext>();

            services.AddDbContext<CrmContext>(config =>
                {
                    config.UseSqlServer(Configuration.GetConnectionString("LocalDb"));
                });

            //TODO: check database for root user
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void AddDependencyInjection(IServiceCollection services)
        {
            //###########################Services#######################################

            services.AddScoped<IAddressService, AddressService>();

            //###########################Repositories#######################################

            services.AddScoped<IAddressRepository, AddressRepository>();
        }
    }
}