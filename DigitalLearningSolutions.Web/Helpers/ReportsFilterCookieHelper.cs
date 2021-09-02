﻿namespace DigitalLearningSolutions.Web.Helpers
{
    using System;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    public static class ReportsFilterCookieHelper
    {
        public static readonly int CookieExpiryDays = 30;
        public static readonly string CookieName = "ReportsFilterCookie";

        public static void SetReportsFilterCookie(
            this IResponseCookies cookies,
            ActivityFilterData filterData,
            DateTime currentDateTime
        )
        {
            var expiry = currentDateTime.AddDays(CookieExpiryDays);
            cookies.Append(
                CookieName,
                JsonConvert.SerializeObject(filterData),
                new CookieOptions
                {
                    Expires = expiry
                }
            );
        }

        public static ActivityFilterData? ParseReportsFilterCookie(this IRequestCookieCollection cookies)
        {
            var cookie = cookies[CookieName];

            return JsonConvert.DeserializeObject<ActivityFilterData>(cookie);
        }
    }
}
