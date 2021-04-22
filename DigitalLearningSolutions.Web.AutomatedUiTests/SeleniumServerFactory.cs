namespace DigitalLearningSolutions.Web.AutomatedUiTests
{
    using System;
    using System.Data;
    using System.Linq;
    using Dapper.FluentMap;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Hosting.Server.Features;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Serilog;

    public class SeleniumServerFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        private IWebHost host;
        public string RootUri;

        public SeleniumServerFactory()
        {
            CreateServer(CreateWebHostBuilder());
        }

        protected sealed override TestServer CreateServer(IWebHostBuilder builder)
        {
            // Real TCP port
            host = builder.Build();
            host.Start();
            RootUri = host.ServerFeatures.Get<IServerAddressesFeature>().Addresses.First();

            // Fake Server to satisfy the return type
            return new TestServer(new WebHostBuilder().UseStartup<TStartup>().UseSerilog());
        }

        protected sealed override IWebHostBuilder CreateWebHostBuilder()
        {
            var builder = WebHost.CreateDefaultBuilder(Array.Empty<string>());
            builder.UseStartup<TStartup>();
            builder.UseSerilog();
            ConfigureWebHost(builder);
            return builder;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                host?.Dispose();
            }
        }

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
