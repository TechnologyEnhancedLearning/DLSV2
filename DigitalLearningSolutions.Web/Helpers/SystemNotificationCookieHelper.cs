namespace DigitalLearningSolutions.Web.Helpers
{
    using System;
    using Microsoft.AspNetCore.Http;

    public static class SystemNotificationCookieHelper
    {
        public static readonly int CookieExpiryHours = 24;
        public static readonly string CookieName = "SkipSystemNotificationsCookie";

        public static void SetSkipSystemNotificationCookie(
            this IResponseCookies cookies,
            int adminId,
            DateTime currentDateTime
        )
        {
            var expiry = currentDateTime.AddHours(CookieExpiryHours);
            cookies.Append(
                CookieName,
                adminId.ToString(),
                new CookieOptions
                {
                    Expires = expiry
                }
            );
        }

        public static void DeleteSkipSystemNotificationCookie(this IResponseCookies cookies)
        {
            cookies.Delete(CookieName);
        }

        public static bool HasSkippedNotificationsCookie(this IRequestCookieCollection cookies, int adminId)
        {
            if (cookies.ContainsKey(CookieName))
            {
                return cookies[CookieName] == adminId.ToString();
            }

            return false;
        }
    }
}
