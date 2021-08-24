namespace DigitalLearningSolutions.Web.Helpers
{
    using System;
    using Microsoft.AspNetCore.Http;

    public static class FilterCookieHelper
    {
        private static readonly DateTimeOffset CookieExpiry = DateTimeOffset.UtcNow.AddDays(30);

        public static void UpdateOrDeleteFilterCookie(this HttpResponse response, string cookieName, string? filterBy)
        {
            if (filterBy != null)
            {
                response.Cookies.Append(
                    cookieName,
                    filterBy,
                    new CookieOptions
                    {
                        Expires = CookieExpiry
                    }
                );
            }
            else
            {
                response.Cookies.Delete(cookieName);
            }
        }
    }
}
