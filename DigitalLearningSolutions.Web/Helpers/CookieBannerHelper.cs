using DigitalLearningSolutions.Data.Models.TrackingSystem;
using DigitalLearningSolutions.Web.ViewModels.Frameworks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;

namespace DigitalLearningSolutions.Web.Helpers
{
    public static class CookieBannerHelper
    {
        public static readonly int CookieExpiryDays = 365;
        public static readonly string CookieName = "Dls-cookie-consent";

        public static void SetDLSBannerCookie(
            this IResponseCookies cookies,
            string value,
            DateTime currentDateTime
        )
        {
            var expiry = currentDateTime.AddDays(CookieExpiryDays);
            cookies.Append(
                CookieName,
                value,
                new CookieOptions
                {
                    Expires = expiry
                }
            );
        }

        public static void DeleteDLSBannerCookie(this IResponseCookies cookies)
        {
            cookies.Delete(CookieName);
        }
    }
}
