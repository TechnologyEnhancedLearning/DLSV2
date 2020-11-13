﻿namespace DigitalLearningSolutions.Web.Helpers
{
    using System;
    using System.IO;
    using Microsoft.Extensions.Configuration;
    using static System.String;

    public static class ConfigHelper
    {
        public const string DefaultConnectionStringName = "DefaultConnection";
        public const string UnitTestConnectionStringName = "UnitTestConnection";

        public static IConfigurationRoot GetAppConfig()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(GetAppSettingsFilename())
                .AddEnvironmentVariables(GetEnvironmentVariablePrefix())
                .Build();
        }
        public static string GetEnvironmentVariablePrefix()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            return $"DlsRefactor{environmentName}_";
        }

        public static string GetAppSettingsFilename()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            return IsNullOrEmpty(environmentName) ? "appsettings.json" : $"appsettings.{environmentName}.json";
        }

        public static string GetEvaluateUrl(this IConfiguration config, int progressId)
        {
            return $"{config["CurrentSystemBaseUrl"]}/tracking/finalise?ProgressID={progressId}&lp=1";
        }
    }
}
