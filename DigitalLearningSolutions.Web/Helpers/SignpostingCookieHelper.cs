using DigitalLearningSolutions.Web.ViewModels.Frameworks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;

namespace DigitalLearningSolutions.Web.Helpers
{
    public static class SignpostingCookieHelper
    {
        public static readonly string CookieName = "SignpostingCookie";
        public static readonly int CookieExpiryDays = 30;

        public static void SetSignpostingCookie(
            this IResponseCookies cookies,
            dynamic data,
            DateTime? currentDateTime = null
        )
        {
            var expiry = (currentDateTime ?? DateTime.UtcNow).AddDays(CookieExpiryDays);
            var settings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };
            cookies.Append(
                CookieName,
                JsonConvert.SerializeObject(data, settings),
                new CookieOptions
                {
                    Expires = expiry
                }
            ); ;
        }

        public static CompetencyResourceSignpostingViewModel RetrieveSignpostingFromCookie(this IRequestCookieCollection cookies)
        {
            try
            {
                var cookie = cookies[CookieName];
                var data = JsonConvert.DeserializeObject<CompetencyResourceSignpostingViewModel>(cookie);
                return data;
            }
            catch
            {
                return null;
            }
        }
    }
}
