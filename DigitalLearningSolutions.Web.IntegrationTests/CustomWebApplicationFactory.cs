namespace DigitalLearningSolutions.Web.IntegrationTests
{
    using System.Data;
    using System.Linq;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup: class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var defaultSqlConnection = services.SingleOrDefault(
                    service => service.ServiceType == typeof(IDbConnection));

                services.Remove(defaultSqlConnection);

                var config = ConfigHelper.GetAppConfig();
                var connectionString = config.GetConnectionString(ConfigHelper.UnitTestConnectionStringName);
                services.AddScoped<IDbConnection>(_ => new SqlConnection(connectionString));
            });
        }
    }
}
