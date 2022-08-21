namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Utilities;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Mvc;

    public static class LoginHelper
    {
        private static readonly IClockUtility ClockUtility = new ClockUtility();

        public static async Task CentrelessLogInAsync(
            this Controller controller,
            UserAccount userAccount,
            bool isPersistent
        )
        {
            var claims = LoginClaimsHelper.GetClaimsForCentrelessSignIn(userAccount);
            var claimsIdentity = new ClaimsIdentity(claims, "Identity.Application");
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = isPersistent,
                IssuedUtc = ClockUtility.UtcNow,
            };

            await controller.HttpContext.SignInAsync(
                "Identity.Application",
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );
        }
    }
}
