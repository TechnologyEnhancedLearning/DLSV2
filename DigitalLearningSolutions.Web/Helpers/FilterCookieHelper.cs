namespace DigitalLearningSolutions.Web.Helpers
{
    using System;
    using DigitalLearningSolutions.Data.Helpers;
    using Microsoft.AspNetCore.Http;

    public static class FilterCookieHelper
    {
        private static readonly DateTimeOffset CookieExpiry = DateTimeOffset.UtcNow.AddDays(30);

        public static void UpdateFilterCookie(
            this HttpResponse response,
            string cookieName,
            string? existingFilterString
        )
        {
            var cookieValue = existingFilterString ?? FilteringHelper.EmptyFiltersCookieValue;

            response.Cookies.Append(
                cookieName,
                cookieValue,
                new CookieOptions
                {
                    Expires = CookieExpiry,
                }
            );
        }
    }
}
