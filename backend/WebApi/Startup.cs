using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using NSwag;
using NSwag.Generation.Processors.Security;
using RepositoryLayer;
using ServiceLayer;
using WebApi.Helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;

            Configuration["dbName"] = "LiveDb";
            if (env.IsDevelopment())
            {
                Configuration["dbName"] = "LocalDb";
            }
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.JwtSecret);

            AddDependencyInjection(services);

            services.AddAutoMapper(c => c.AddProfile<MappingProfile>(), typeof(Startup));

            services.AddSwaggerDocument(settings =>
            {
                settings.Title = "CRohM-API";
                settings.DocumentProcessors.Add(new SecurityDefinitionAppender("JWT token",
                    new OpenApiSecurityScheme()
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        Description = "Copy 'Bearer ' + valid JWT token into field",
                        In = OpenApiSecurityApiKeyLocation.Header,
                    }));
                settings.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT token"));
            });

            services.AddIdentity<User, Role>(options =>
                {
                    //TODO: change password settings in next sprint
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
                config.UseSqlServer(Configuration.GetConnectionString(Configuration["dbName"]));
            });

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "wwwroot";
            });

            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserService userService)
        {
            ApplicationDbInitializer.SeedUsers(userService);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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

            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }
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