namespace DigitalLearningSolutions.Web.Helpers
{
    using System;
    using Microsoft.Extensions.Configuration;

    public static class ContentUrlHelper
    {
        public static string GetContentPath(IConfiguration config, string videoPath)
        {
            if (Uri.IsWellFormedUriString(videoPath, UriKind.Absolute))
            {
                return videoPath;
            }

            var urlWithProtocol = $"https://{videoPath}";
            if (Uri.IsWellFormedUriString(urlWithProtocol, UriKind.Absolute))
            {
                return urlWithProtocol;
            }
            return config["CurrentSystemBaseUrl"] + videoPath;
        }

        public static string? GetNullableContentPath(IConfiguration config, string? videoPath) =>
            videoPath != null ? GetContentPath(config, videoPath) : null;

        public static string ReplaceUrlSegment(string sourceUrl)
        {
            string errorSegment = "LearningSolutions/StatusCode/410";
            string welcomeSegment = "Home/Welcome";

            if (sourceUrl.Contains(errorSegment))
            {
                return sourceUrl.Replace(errorSegment, welcomeSegment);
            }
            return sourceUrl;
        }
    }
}
