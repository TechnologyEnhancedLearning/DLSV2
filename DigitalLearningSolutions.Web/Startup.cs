namespace DigitalLearningSolutions.Web
{
    using System.Data;
    using System.IO;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Factories;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using FluentMigrator.Runner;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.FeatureManagement;
    using Serilog;

    public class Startup
    {
        private readonly IConfiguration config;
        private readonly bool isDevelopment;

        public Startup(IConfiguration config, IHostEnvironment env)
        {
            this.config = config;
            isDevelopment = env.IsDevelopment();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo("C:\\keys"))
                .SetApplicationName("DLSSharedCookieApp");

            services.AddAuthentication("Identity.Application")
                .AddCookie("Identity.Application", options =>
                {
                    options.Cookie.Name = ".AspNet.SharedCookie";
                    options.Events.OnRedirectToLogin = (context) =>
                    {
                        context.HttpContext.Response.Redirect($"{config["CurrentSystemBaseUrl"]}/home?action=login&app=lp");
                        return Task.CompletedTask;
                    };
                });

            services.ConfigureApplicationCookie(options => {
                options.Cookie.Name = ".AspNet.SharedCookie";
            });

            services.AddFeatureManagement();
            var mvcBuilder = services.AddControllersWithViews();
            if (isDevelopment)
            {
                mvcBuilder.AddRazorRuntimeCompilation();
            }

            var defaultConnectionString = config["ConnectionStrings:DefaultConnection"];

            // Register database migration runner.
            services.RegisterMigrationRunner(defaultConnectionString);

            // Register database connection for Dapper.
            services.AddScoped<IDbConnection>(_ => new SqlConnection(defaultConnectionString));

            // Register data services.
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IUnlockDataService, UnlockDataService>();
            services.AddScoped<IConfigService, ConfigService>();
            services.AddScoped<IUnlockService, UnlockService>();
            services.AddScoped<ISmtpClientFactory, SmtpClientFactory>();
        }

        public void Configure(IApplicationBuilder app, IMigrationRunner migrationRunner, IFeatureManager featureManager)
        {
            if (isDevelopment)
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }

            app.UseExceptionHandler("/LearningPortal/Error");
            app.UseStatusCodePagesWithReExecute("/LearningPortal/StatusCode/{0}");
            app.UseStaticFiles();
            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(async (endpoints) => await ConfigureEndPointsAsync(endpoints, featureManager));

            migrationRunner.MigrateUp();
        }

        private async System.Threading.Tasks.Task ConfigureEndPointsAsync(IEndpointRouteBuilder endpoints, IFeatureManager featureManager)
        {
            if (await featureManager.IsEnabledAsync(nameof(FeatureFlags.Login)))
            {
                endpoints.MapControllerRoute("default", "{controller=Login}/{action=Index}");
            }
            else
            {
                endpoints.MapControllerRoute("default", "{controller=LearningPortal}/{action=Current}");
            }
        }
    }
}
