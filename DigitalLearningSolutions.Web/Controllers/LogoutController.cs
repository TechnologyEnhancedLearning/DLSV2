﻿namespace DigitalLearningSolutions.Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.OpenIdConnect;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using DigitalLearningSolutions.Data.Extensions;

    public class LogoutController : Controller
    {
        private readonly IConfiguration config;

        public LogoutController(
            IConfiguration config)
        {
            this.config = config;
        }

        [HttpPost]
        public async Task<IActionResult> Index()
        {
            var appRootPath = config.GetAppRootPath();
            if (!(this.User?.Identity.IsAuthenticated ?? false))
            {
                return this.RedirectToAction(appRootPath + "/home");
            }

            string? authScheme = string.Empty;
            HttpContext.Request.Cookies.TryGetValue(
                "auth_method",
                out authScheme);

            await HttpContext.SignOutAsync("Identity.Application");

            if (string.IsNullOrEmpty(authScheme))
            {
                return this.Redirect(appRootPath + "/home");
            }

            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
            return LogoutExternalProvider();
        }

        public IActionResult LogoutSharedAuth()
        {
            return LogoutExternalProvider();
        }

        private IActionResult LogoutExternalProvider()
        {
            var idToken = string.Empty;
            HttpContext.Request.Cookies.TryGetValue(
                "id_token",
                out idToken);
            var auth = config.GetLearningHubAuthenticationAuthority();
            var appRootPath = config.GetAppRootPath();
            var uri = Uri.EscapeDataString($"{appRootPath}/signout-callback-oidc");
            var logoutUrl = $"{auth}/connect/endsession" +
                $"?post_logout_redirect_uri={uri}" +
                $"&id_token_hint={idToken}";
            HttpContext.Response.Cookies.Delete("auth_method");
            HttpContext.Response.Cookies.Delete("id_token");
            return Redirect(logoutUrl);
        }

    }
}
