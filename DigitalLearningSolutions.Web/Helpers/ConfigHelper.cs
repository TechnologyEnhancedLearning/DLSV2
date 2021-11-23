namespace DigitalLearningSolutions.Web.Helpers
{
    using System;
    using System.IO;
    using Microsoft.Extensions.Configuration;
    using static System.String;

    [Obsolete(
        "We are moving to the ConfigHelper in the Data project. Copy the methods you need into there. This class will be removed in HEEDLS-609"
    )]
    public static class ConfigHelper
    {
        public const string DefaultConnectionStringName = "DefaultConnection";
        public const string UnitTestConnectionStringName = "UnitTestConnection";
        public const string CurrentSystemBaseUrlName = "CurrentSystemBaseUrl";
        public const string AppRootPathName = "AppRootPath";
        public const string MapsApiKey = "MapsAPIKey";
        public const string LearningHubApiKey = "LearningHubAPIKey";

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

        public static string GetEvaluateUrl(this IConfiguration config, int progressId, bool fromLearningPortal)
        {
            return $"{config[CurrentSystemBaseUrlName]}/tracking/finalise?ProgressID={progressId}";
        }

        public static string GetTrackingUrl(this IConfiguration config)
        {
            return $"{config[CurrentSystemBaseUrlName]}/tracking/tracker";
        }

        public static string GetScormPlayerUrl(this IConfiguration config)
        {
            return $"{config[CurrentSystemBaseUrlName]}/scoplayer/sco";
        }

        public static string GetDownloadSummaryUrl(this IConfiguration config, int progressId)
        {
            return $"{config[CurrentSystemBaseUrlName]}/tracking/summary?ProgressID={progressId}";
        }

        public static string GetConsolidationPathUrl(this IConfiguration config, string consolidationPath)
        {
            return $"{config[CurrentSystemBaseUrlName]}/tracking/dlconsolidation?client={consolidationPath}";
        }

        public static string GetMapsApiKey(this IConfiguration config)
        {
            return config[MapsApiKey];
        }

        public static string GetAppRootPath(this IConfiguration config)
        {
            return config[AppRootPathName];
        }

        public static string GetLearningHubApiKey(this IConfiguration config)
        {
            return config[LearningHubApiKey];
        }
    }
}
