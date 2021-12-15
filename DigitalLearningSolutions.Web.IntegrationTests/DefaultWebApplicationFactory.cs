namespace DigitalLearningSolutions.Web.IntegrationTests
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;

    public class DefaultWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected sealed override IWebHostBuilder CreateWebHostBuilder()
        {
            return base.CreateWebHostBuilder().UseEnvironment("SIT");
        }
    }
}
