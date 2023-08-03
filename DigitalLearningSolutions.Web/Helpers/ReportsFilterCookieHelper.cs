﻿namespace DigitalLearningSolutions.Web.Helpers
{
    using System;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    public static class ReportsFilterCookieHelper
    {
        public static readonly int CookieExpiryDays = 30;

        public static void SetReportsFilterCookie(
            this IResponseCookies cookies,
            string? cookieName,
            ActivityFilterData filterData,
            DateTime currentDateTime
        )
        {
            var expiry = currentDateTime.AddDays(CookieExpiryDays);
            cookies.Append(
                cookieName?? "ReportsFilterCookie",
                JsonConvert.SerializeObject(filterData),
                new CookieOptions
                {
                    Expires = expiry
                }
            );
        }

        public static ActivityFilterData RetrieveFilterDataFromCookie(this IRequestCookieCollection cookies, string? cookieName, int? categoryIdFilter)
        {
            var cookie = cookies[cookieName ?? "ReportsFilterCookie"];

            if (cookie == null)
            {
                return ActivityFilterData.GetDefaultFilterData(categoryIdFilter);
            }

            try
            {
                var filterData = JsonConvert.DeserializeObject<ActivityFilterData>(cookie);
                if (categoryIdFilter != null)
                {
                    filterData.CourseCategoryId = categoryIdFilter;
                }

                return filterData;
            }
            catch
            {
                return ActivityFilterData.GetDefaultFilterData(categoryIdFilter);
            }
        }
    }
}
