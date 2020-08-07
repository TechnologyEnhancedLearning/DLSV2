namespace DigitalLearningSolutions.Web
{
    using System;
    using System.IO;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Serilog;
    using Serilog.Events;
    using Serilog.Sinks.MSSqlServer.Sinks.MSSqlServer.Options;

    public class Program
    {
        public static void Main(string[] args)
        {
            SetUpLogger();
            try
            {
                Log.Information("Starting up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddEnvironmentVariables(prefix: GetEnvironmentVariablePrefix());
                })
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
        }

        public static void SetUpLogger()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(GetAppSettingsFilename())
                .AddEnvironmentVariables(GetEnvironmentVariablePrefix())
                .Build();
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.MSSqlServer(
                    connectionString: config["ConnectionStrings:DefaultConnection"],
                    sinkOptions: new SinkOptions { TableName = "V2LogEvents", AutoCreateSqlTable = true },
                    appConfiguration: config)
                .CreateLogger();
        }

        private static string GetEnvironmentVariablePrefix()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            return $"DlsRefactor{environmentName}_";
        }

        private static string GetAppSettingsFilename()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            return environmentName == Environments.Production ? "appsettings.json" : $"appsettings.{environmentName}.json";
        }
    }
}
