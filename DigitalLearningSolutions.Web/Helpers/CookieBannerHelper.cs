using DigitalLearningSolutions.Data.Models.TrackingSystem;
using DigitalLearningSolutions.Web.ViewModels.Frameworks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;

namespace DigitalLearningSolutions.Web.Helpers
{
    public static class CookieBannerHelper
    {
        //public static readonly int CookieExpiryDays = 365;
        //public static readonly string CookieName = "Dls-cookie-consent";

        public static void SetDLSBannerCookie(
            this IResponseCookies cookies,
            string cookieName,
            string value,
            DateTime expiry
        )
        {
            //var expiry = currentDateTime.AddDays(CookieExpiryDays);
            cookies.Append(
                cookieName,
                value,
                new CookieOptions
                {
                    Expires = expiry
                }
            );
        }

        public static bool HasDLSBannerCookie(this IRequestCookieCollection cookies, string cookieName, string value)
        {
            if (cookies.ContainsKey(cookieName))
            {
                return cookies[cookieName] == value;
            }

            return false;
        }       
    }
}
