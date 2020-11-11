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
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;
using ModelLayer.Helper;
using System.Net.Mail;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var server = Configuration["DBServer"] ?? "localhost";
            var port = Configuration["DBPort"] ?? "1433";
            var user = Configuration["DBUser"] ?? "SA";
            var password = Configuration["DBPassword"] ?? "CRohM2020";
            var database = Configuration["DBName"] ?? "CRMDB"; 

            var connectionString = $"Server={server},{port};Database={database};User Id={user};Password={password}";
            connectionString = "Server=.\\SQLEXPRESS;Database=CRMDB;Trusted_Connection=True;";
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
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
                    //// Password settings
                    options.Password.RequireDigit = PasswordGuidelines.RequireDigit;
                    options.Password.RequireLowercase = PasswordGuidelines.RequireLowercase;
                    options.Password.RequireNonAlphanumeric = PasswordGuidelines.RequireNonAlphanumeric;
                    options.Password.RequireUppercase = PasswordGuidelines.RequireUppercase;
                    options.Password.RequiredLength = PasswordGuidelines.RequiredMinLength;
                    options.Password.RequiredUniqueChars = PasswordGuidelines.RequiredUniqueChars;
                    options.User.RequireUniqueEmail = true;
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    options.Lockout.MaxFailedAccessAttempts = 3;
                    options.Lockout.AllowedForNewUsers = true;
                })
                .AddEntityFrameworkStores<CrmContext>()
                .AddUserManager<UserManager<User>>()
                .AddSignInManager<SignInManager<User>>();

            services.AddDbContext<CrmContext>(config =>
            {
                config.UseSqlServer(connectionString);
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

            services.AddHealthChecks();

        }

        public void Configure(
            IRoleService rolesService,
            IMapper mapper,
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IUserService userService,
            CrmContext dataContext,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            dataContext.Database.Migrate();
            ApplicationDbInitializer.SeedRoles(rolesService);
            ApplicationDbInitializer.SeedUsers(userService);
            MailCredentialsHelper.CheckIfCredentialsExist(
                new MailCredentials(
                    new MailAddress(
                        Configuration["MailAddress"] ?? "a@b.com",
                        Configuration["MailDisplayName"] ?? "CRMS-Team"),
                        new System.Net.NetworkCredential(Configuration["MailUserName"] ?? "c@d.com",
                        Configuration["MailPassword"] ?? "password"),
                        int.Parse(Configuration["MailPort"] ?? "587"),
                        Configuration["MailHost"] ?? "smtp.a.com"));
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
                endpoints.MapHealthChecks("/health");
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
            new UserCheckThread(serviceProvider.CreateScope().ServiceProvider.GetService<IUserCheckDateService>(), serviceProvider.CreateScope().ServiceProvider.GetService<IContactCheckDateService>(), configuration).runScheduledService().Wait();
        }

        private void AddDependencyInjection(IServiceCollection services)
        {
            //###########################Helper#######################################

            services.AddSingleton<IMailService, MailService>();

            //###########################Services#######################################
            services.AddScoped<RoleManager<Role>>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserManager, DefaultUserManager>();
            services.AddScoped<ISignInService, SignInService>();
            services.AddScoped<ISignInManager, DefaultSignInManager>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IOrganizationService, OrganizationService>();
            services.AddScoped<IEducationalOpportunityService, EducationalOpportunityService>();
            services.AddScoped<IContactService, ContactService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IUserCheckDateService, UserCheckDateService>();
            services.AddScoped<IContactCheckDateService, ContactCheckDateService>();
            services.AddScoped<IModificationEntryService, ModificationEntryService>();
            services.AddScoped<IUserLoginService, UserLoginService>();
            services.AddScoped<IDataProtectionService, DataProtectionService>();
            services.AddScoped<IHistoryService, HistoryService>();
            services.AddScoped<IStatisticsService, StatisticsService>();
            //###########################Repositories#######################################

            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IEducationalOpportunityRepository, EducationalOpportunityRepository>();
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<IOrganizationContactRepository, OrganizationContactRepository>();
            services.AddScoped<IEventContactRepository, EventContactRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IModificationEntryRepository, ModificationEntryRepository>();
            services.AddScoped<IUserCheckDateRepository, UserCheckDateRepository>();
            services.AddScoped<IContactPossibilitiesEntryRepository, ContactPossibilitiesEntryRepository>();
            services.AddScoped<IUserLoginRepository, UserLoginRepository>();
            services.AddScoped<IEventOrganizationRepository, EventOrganizationRepository>();
            services.AddScoped<IHistoryRepository, HistoryRepository>();
        }
    }
}
