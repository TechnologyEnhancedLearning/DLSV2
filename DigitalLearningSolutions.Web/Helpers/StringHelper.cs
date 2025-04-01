namespace DigitalLearningSolutions.Web.Helpers
{
    using System;
    using System.Text.RegularExpressions;
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
        public static string StripHtmlTags(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            // Remove HTML tags
            string result = Regex.Replace(input, "<.*?>", string.Empty).Trim();

            return string.IsNullOrEmpty(result) ? string.Empty : result;
        }
    }
}
