﻿namespace DigitalLearningSolutions.Data.Helpers
{
    using Microsoft.Extensions.Configuration;

    public static class ConfigHelper
    {
        public const string AppRootPathName = "AppRootPath";
        public const string CurrentSystemBaseUrlName = "CurrentSystemBaseUrl";
        private const string LearningHubApiKey = "LearningHubApiKey";

        public static string GetAppRootPath(this IConfiguration config)
        {
            return config[AppRootPathName];
        }

        public static string GetCurrentSystemBaseUrl(this IConfiguration config)
        {
            return config[CurrentSystemBaseUrlName];
        }

        public static string GetLearningHubApiKey(this IConfiguration config)
        {
            return config[LearningHubApiKey];
        }
    }
}
