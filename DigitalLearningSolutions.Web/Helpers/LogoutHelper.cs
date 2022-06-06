namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;

    public static class LogoutHelper
    {
        public static async Task Logout(this HttpContext httpContext)
        {
            await httpContext.SignOutAsync();
            httpContext.Response.Cookies.Delete("ASP.NET_SessionId");
        }
    }
}
