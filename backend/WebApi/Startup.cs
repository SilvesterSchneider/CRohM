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
using ModelLayer.Helper;

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
                    //// Password settings.
                    options.Password.RequireDigit = PasswordGuidelines.RequireDigit;
                    options.Password.RequireLowercase = PasswordGuidelines.RequireLowercase;
                    options.Password.RequireNonAlphanumeric = PasswordGuidelines.RequireNonAlphanumeric;
                    options.Password.RequireUppercase = PasswordGuidelines.RequireUppercase;
                    options.Password.RequiredLength = PasswordGuidelines.RequiredLength;
                    options.Password.RequiredUniqueChars = PasswordGuidelines.RequiredUniqueChars;
                    options.User.RequireUniqueEmail = true;
                    options.SignIn.RequireConfirmedAccount = false;
                })
                .AddEntityFrameworkStores<CrmContext>()
                .AddUserManager<UserManager<User>>()
                .AddSignInManager<SignInManager<User>>();

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

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IUserService userService, CrmContext dataContext)
        {
            dataContext.Database.Migrate();

            ApplicationDbInitializer.SeedUsers(userService);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
            //###########################Helper#######################################

            services.AddSingleton<IMailProvider, MailProviderTest>();

            //###########################Services#######################################

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserManager, DefaultUserManager>();
            services.AddScoped<ISignInService, SignInService>();
            services.AddScoped<ISignInManager, DefaultSignInManager>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IOrganizationService, OrganizationService>();
            services.AddScoped<IEducationalOpportunityService, EducationalOpportunityService>();

            //###########################Repositories#######################################

            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IEducationalOpportunityRepository, EducationalOpportunityRepository>();
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        }
    }
}