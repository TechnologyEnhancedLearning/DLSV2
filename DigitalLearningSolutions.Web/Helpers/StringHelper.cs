namespace DigitalLearningSolutions.Web.Helpers
{
    using System;
    using DigitalLearningSolutions.Data.Extensions;
    using Microsoft.Extensions.Configuration;

    public static class StringHelper
    {
        public static bool StringsMatchCaseInsensitive(string? firstString, string? secondString)
        {
            return string.Equals(firstString, secondString, StringComparison.CurrentCultureIgnoreCase);
        }

        public static string GetLocalRedirectUrl(IConfiguration config, string basicUrl)
        {
            var applicationPath = new Uri(config.GetAppRootPath()).AbsolutePath.TrimEnd('/');
            return applicationPath + basicUrl;
        }
    }
}
