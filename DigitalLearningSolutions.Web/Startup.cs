namespace DigitalLearningSolutions.Web
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Startup
    {
        private readonly bool isDevelopment;

        public Startup(IWebHostEnvironment env)
        {
            isDevelopment = env.IsDevelopment();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var mvcBuilder = services.AddControllersWithViews();
            if (isDevelopment)
            {
                mvcBuilder.AddRazorRuntimeCompilation();
            }
        }

        public void Configure(IApplicationBuilder app)
        {
            if (isDevelopment)
            {
                app.UseDeveloperExceptionPage();
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
