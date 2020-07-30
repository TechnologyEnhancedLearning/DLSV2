namespace DigitalLearningSolutions.Web
{
    using System.Data;
    using DigitalLearningSolutions.Data.Services;
    using FluentMigrator.Runner;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Serilog;

    public class Startup
    {
        private readonly IConfiguration config;
        private readonly bool isDevelopment;

        public Startup(IConfiguration config, IWebHostEnvironment env)
        {
            this.config = config;
            isDevelopment = env.IsDevelopment();
        }

        public void ConfigureServices(IServiceCollection services)
        {
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
        }

        public void Configure(IApplicationBuilder app, IMigrationRunner migrationRunner)
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
            app.UseEndpoints(ConfigureEndPoints);

            migrationRunner.MigrateUp();
        }

        private void ConfigureEndPoints(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllerRoute("default", "{controller=LearningPortal}/{action=Current}");
        }
    }
}
