namespace DigitalLearningSolutions.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Login;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
    [SetSelectedTab(nameof(NavMenuTab.LogIn))]
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> logger;
        private readonly ILoginService loginService;
        private readonly ISessionService sessionService;
        private readonly IUserService userService;

        public LoginController(
            ILoginService loginService,
            ISessionService sessionService,
            ILogger<LoginController> logger,
            IUserService userService
        )
        {
            this.loginService = loginService;
            this.sessionService = sessionService;
            this.logger = logger;
            this.userService = userService;
        }

        public IActionResult Index(string? returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new LoginViewModel(returnUrl);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var loginResult = loginService.AttemptLogin(model.Username!.Trim(), model.Password!);
            switch (loginResult.LoginAttemptResult)
            {
                case LoginAttemptResult.InvalidUsername:
                    ModelState.AddModelError(
                        "Username",
                        "A user with this email or delegate ID could not be found"
                    );
                    return View("Index", model);
                case LoginAttemptResult.InvalidPassword:
                    ModelState.AddModelError("Password", "The password you have entered is incorrect");
                    return View("Index", model);
                case LoginAttemptResult.AccountsHaveMismatchedPasswords:
                    return View("MismatchingPasswords");
                case LoginAttemptResult.AccountLocked:
                    return View("AccountLocked", loginResult.UserEntity!.UserAccount.FailedLoginCount);
                case LoginAttemptResult.InactiveAccount:
                    return View("AccountInactive");
                case LoginAttemptResult.LogIntoSingleCentre:
                    return await LogIntoCentreAsync(
                        loginResult.UserEntity!,
                        model.RememberMe,
                        model.ReturnUrl,
                        loginResult.CentreToLogInto!.Value
                    );
                case LoginAttemptResult.ChooseACentre:
                    await CentrelessLogInAsync(loginResult.UserEntity!, model.RememberMe);
                    return RedirectToAction("ChooseACentre", "Login", new { returnUrl = model.ReturnUrl });
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [HttpGet]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        [Authorize(Policy = CustomPolicies.BasicUser)]
        public IActionResult ChooseACentre(string? returnUrl)
        {
            // TODO HEEDLS-912: sort out ChooseACentre page
            var model = new ChooseACentreViewModel(new List<ChooseACentreAccount>());
            return View("ChooseACentre", model);
        }

        [HttpGet]
        [Authorize(Policy = CustomPolicies.BasicUser)]
        public async Task<IActionResult> ChooseCentre(int centreId, string? returnUrl)
        {
            // TODO HEEDLS-912: sort out ChooseACentre page
            var rememberMe = true;
            var userEntity = userService.GetUserById(User.GetUserIdKnownNotNull());
            var firstAdminAccountCentreId = userEntity!.AdminAccounts.FirstOrDefault()?.CentreId;
            var firstDelegateAccountCentreId = userEntity.DelegateAccounts.FirstOrDefault()?.CentreId;
            var tempCentreIdToLogInto = (firstAdminAccountCentreId ?? firstDelegateAccountCentreId)!.Value;
            return await LogIntoCentreAsync(userEntity!, rememberMe, returnUrl, tempCentreIdToLogInto);
        }

        private async Task<IActionResult> LogIntoCentreAsync(
            UserEntity userEntity,
            bool rememberMe,
            string? returnUrl,
            int centreIdToLogInto
        )
        {
            var claims = LoginClaimsHelper.GetClaimsForSignIntoCentre(userEntity, centreIdToLogInto);
            var claimsIdentity = new ClaimsIdentity(claims, "Identity.Application");
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = rememberMe,
                IssuedUtc = DateTime.UtcNow,
            };

            var adminAccount = userEntity!.AdminAccounts.SingleOrDefault(aa => aa.CentreId == centreIdToLogInto);
            if (adminAccount != null)
            {
                sessionService.StartAdminSession(adminAccount.Id);
            }

            await HttpContext.SignInAsync("Identity.Application", new ClaimsPrincipal(claimsIdentity), authProperties);

            if (!userService.ShouldForceDetailsCheck(userEntity, centreIdToLogInto))
            {
                return this.RedirectToReturnUrl(returnUrl, logger) ?? RedirectToAction("Index", "Home");
            }

            const bool isCheckDetailsRedirect = true;
            if (returnUrl == null)
            {
                return RedirectToAction("EditDetails", "MyAccount", new { isCheckDetailsRedirect });
            }

            var dlsSubAppSection = returnUrl.Split('/')[1];
            DlsSubApplication.TryGetFromUrlSegment(dlsSubAppSection, out var dlsSubApplication);
            return RedirectToAction(
                "EditDetails",
                "MyAccount",
                new { returnUrl, dlsSubApplication, isCheckDetailsRedirect }
            );
        }

        private async Task CentrelessLogInAsync(UserEntity userEntity, bool rememberMe)
        {
            var claims = LoginClaimsHelper.GetClaimsForCentrelessSignIn(userEntity.UserAccount);
            var claimsIdentity = new ClaimsIdentity(claims, "Identity.Application");
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = rememberMe,
                IssuedUtc = DateTime.UtcNow,
            };

            await HttpContext.SignInAsync("Identity.Application", new ClaimsPrincipal(claimsIdentity), authProperties);
        }
    }
}
