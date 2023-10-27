namespace DigitalLearningSolutions.Web.Helpers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Models.Enums;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Mvc;
    using DigitalLearningSolutions.Web.Services;

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

        public static async Task CentrelessLogInAsync(
           TicketReceivedContext context,
           UserAccount userAccount,
           bool isPersistent
       )
        {
            var claims = LoginClaimsHelper.GetClaimsForCentrelessSignIn(userAccount);
            var claimsIdentity = new ClaimsIdentity(
                claims,
                "Identity.Application");
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = isPersistent,
                IssuedUtc = ClockUtility.UtcNow,
            };

            await context.HttpContext.SignInAsync(
                "Identity.Application",
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );
        }

        public static async Task<string> LogIntoCentreAsync(
           UserEntity userEntity,
           bool rememberMe,
           string? returnUrl,
           int centreIdToLogInto,
           TicketReceivedContext context,
           ISessionService sessionService,
           IUserService userService
       )
        {
            var claims = LoginClaimsHelper.GetClaimsForSignIntoCentre(
                userEntity,
                centreIdToLogInto);
            var claimsIdentity = new ClaimsIdentity(
                claims,
                "Identity.Application");
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = rememberMe,
                IssuedUtc = DateTimeOffset.UtcNow,
            };

            var adminAccount = userEntity!
                .GetCentreAccountSet(centreIdToLogInto)?
                .AdminAccount;

            if (adminAccount?.Active == true)
            {
                sessionService.StartAdminSession(adminAccount.Id);
            }

            await context.HttpContext.SignInAsync(
                "Identity.Application",
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (centreIdToLogInto <= 0)
            {
                return "/MyAccount/Index";
            }

            if (!userService.ShouldForceDetailsCheck(
                userEntity,
                centreIdToLogInto))
            {
                if (returnUrl != null)
                {
                    return returnUrl;
                }
                return "/Home/Index";
            }

            if (returnUrl == null)
            {
                return "/MyAccount/EditDetails";
            }

            var dlsSubAppSection = returnUrl.Split('/')[1];
            DlsSubApplication.TryGetFromUrlSegment(
                dlsSubAppSection,
                out var dlsSubApplication);
            return "/MyAccount/EditDetails";
        }
    }
}
