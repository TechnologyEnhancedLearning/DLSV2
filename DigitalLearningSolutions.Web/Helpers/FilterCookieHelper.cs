namespace DigitalLearningSolutions.Web.Helpers
{
    using System;
    using Microsoft.AspNetCore.Http;

    public static class FilterCookieHelper
    {
        private static readonly DateTimeOffset CookieExpiry = DateTimeOffset.UtcNow.AddDays(30);

        public static void UpdateOrDeleteFilterCookie(this HttpResponse response, string cookieName, string? existingFilterString)
        {
            if (existingFilterString != null)
            {
                response.Cookies.Append(
                    cookieName,
                    existingFilterString,
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
