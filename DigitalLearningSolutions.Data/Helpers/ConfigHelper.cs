namespace DigitalLearningSolutions.Data.Helpers
{
    using Microsoft.Extensions.Configuration;

    public static class ConfigHelper
    {
        public const string AppRootPathName = "AppRootPath";

        public static string GetAppRootPath(this IConfiguration config)
        {
            return config[AppRootPathName];
        }
    }
}
