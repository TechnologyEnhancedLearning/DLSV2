namespace DigitalLearningSolutions.Web
{
    using System.Data;
    using System.Reflection;
    using DigitalLearningSolutions.Data.Services;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

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

            services.AddScoped<IDbConnection>(_ => new SqlConnection(config["ConnectionStrings:DefaultConnection"]));

            // Register data services
            services.AddScoped<IHeadlineFiguresService, HeadlineFiguresService>();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (isDevelopment)
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(ConfigureEndPoints);
        }

        private void ConfigureEndPoints(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
        }
    }
}
