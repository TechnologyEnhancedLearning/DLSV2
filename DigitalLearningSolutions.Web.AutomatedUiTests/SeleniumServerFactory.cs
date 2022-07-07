namespace DigitalLearningSolutions.Web.AutomatedUiTests
{
    using System;
    using System.IO;
    using System.Linq;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Hosting.Server.Features;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Configuration.Json;
    using Serilog;

    public class SeleniumServerFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        private IWebHost host;
        public string RootUri;

        public SeleniumServerFactory()
        {
            CreateServer(CreateWebHostBuilder());
            CreateClient();
        }

        protected sealed override TestServer CreateServer(IWebHostBuilder builder)
        {
            // Real TCP port
            host = builder.Build();
            host.Start();
            RootUri = host.ServerFeatures.Get<IServerAddressesFeature>().Addresses.First();

            // Fake Server to satisfy the return type
            return new TestServer(
                new WebHostBuilder()
                    .UseStartup<TStartup>()
                    .UseSerilog()
                    .ConfigureAppConfiguration(
                        configBuilder => { configBuilder.AddConfiguration(GetConfigForUiTests()); }
                    )
            );
        }

        protected sealed override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder(Array.Empty<string>())
                .UseStartup<TStartup>()
                .UseSerilog()
                .UseUrls("http://127.0.0.1:0")
                .ConfigureAppConfiguration(
                    configBuilder =>
                    {
                        var jsonConfigSources = configBuilder.Sources
                            .Where(source => source.GetType() == typeof(JsonConfigurationSource))
                            .ToList();

                        foreach (var jsonConfigSource in jsonConfigSources)
                        {
                            configBuilder.Sources.Remove(jsonConfigSource);
                        }

                        configBuilder.AddConfiguration(GetConfigForUiTests());
                    }
                );
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                host?.Dispose();
            }
        }

        private static IConfigurationRoot GetConfigForUiTests()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.SIT.json")
                .AddEnvironmentVariables("DlsRefactor_")
                .AddUserSecrets(typeof(Startup).Assembly)
                .Build();
        }
    }
}
