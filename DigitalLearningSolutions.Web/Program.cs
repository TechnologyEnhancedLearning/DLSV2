namespace DigitalLearningSolutions.Web
{
    using System;
    using DigitalLearningSolutions.Web.Helpers;
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
                    config.AddEnvironmentVariables(prefix: ConfigHelper.GetEnvironmentVariablePrefix());
                })
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
        }

        public static void SetUpLogger()
        {
            var config = ConfigHelper.GetAppConfig();
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.MSSqlServer(
                    connectionString: config.GetConnectionString(ConfigHelper.DefaultConnectionStringName),
                    sinkOptions: new SinkOptions { TableName = "V2LogEvents", AutoCreateSqlTable = true },
                    appConfiguration: config, restrictedToMinimumLevel: LogEventLevel.Error)
                .CreateLogger();
        }
    }
}
