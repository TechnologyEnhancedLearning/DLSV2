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
    }
}
