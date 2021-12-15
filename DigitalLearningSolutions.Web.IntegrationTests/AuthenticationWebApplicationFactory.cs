namespace DigitalLearningSolutions.Web.IntegrationTests
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Web.IntegrationTests.TestHelpers;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Extensions.DependencyInjection;

    public class AuthenticationWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected sealed override IWebHostBuilder CreateWebHostBuilder()
        {
            return base.CreateWebHostBuilder();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder
                .UseEnvironment("SIT")
                .ConfigureServices(
                    services =>
                    {
                        services.Configure<MvcOptions>(
                            o => { o.Filters.Remove(new AutoValidateAntiforgeryTokenAttribute()); }
                        );

                        services.AddTransient<IStartupFilter, TestUserAppStartupFilter>();

                        var userDataServiceDescriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(IUserDataService)
                        );
                        services.Remove(userDataServiceDescriptor);
                        services.AddScoped(x => TestUserDataService.FakeUserDataService());
                    }
                );
        }
    }
}
