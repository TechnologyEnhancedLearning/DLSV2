namespace DigitalLearningSolutions.Web.Helpers
{
    using Microsoft.AspNetCore.Http;

    public static class UserIpHelper
    {
        public static string GetUserIpAddressFromRequest(this HttpRequest request)
        {
            return request.Headers.ContainsKey("X-Forwarded-For") ?
                request.Headers["X-Forwarded-For"].ToString() :
                request.HttpContext.Connection.RemoteIpAddress.ToString();
        }
    }
}
