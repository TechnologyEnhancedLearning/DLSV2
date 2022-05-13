namespace DigitalLearningSolutions.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.Login;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
    [SetSelectedTab(nameof(NavMenuTab.LogIn))]
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> logger;
        private readonly ILoginService loginService;
        private readonly ISessionService sessionService;

        public LoginController(
            ILoginService loginService,
            ISessionService sessionService,
            ILogger<LoginController> logger
        )
        {
            this.loginService = loginService;
            this.sessionService = sessionService;
            this.logger = logger;
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
                    ModelState.AddModelError("Username", "A user with this email address or delegate ID could not be found");
                    return View("Index", model);
                case LoginAttemptResult.InvalidPassword:
                    ModelState.AddModelError("Password", "The password you have entered is incorrect");
                    return View("Index", model);
                case LoginAttemptResult.AccountLocked:
                    return RedirectToAction("AccountLocked", new { failedCount = loginResult.UserEntity!.UserAccount.FailedLoginCount });
                case LoginAttemptResult.AccountNotApproved:
                    return View("AccountNotApproved");
                case LoginAttemptResult.InactiveCentre:
                    return View("CentreInactive");
                case LoginAttemptResult.AccountsHaveMismatchedPasswords:
                    return View("MismatchingPasswords");
                case LoginAttemptResult.InactiveAccount:
                    return View("AccountInactive");
                case LoginAttemptResult.LogIntoSingleCentre:
                    sessionService.StartAdminSession(loginResult.UserEntity!.AdminAccounts.FirstOrDefault()?.Id);
                    return await LogIn(
                        loginResult.UserEntity,
                        model.RememberMe,
                        model.ReturnUrl
                    );
                case LoginAttemptResult.ChooseACentre:
                    var chooseACentreViewModel = new ChooseACentreViewModel(loginResult.AvailableCentres, loginResult.UserEntity!.IsLocked);
                    SetTempDataForChooseACentre(
                        model.RememberMe,
                        loginResult.UserEntity!,
                        chooseACentreViewModel,
                        model.ReturnUrl
                    );
                    return RedirectToAction("ChooseACentre", "Login");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // TODO Change the Service Filter to be dependent on the new user entity
        [ServiceFilter(typeof(RedirectEmptySessionData<List<CentreUserDetails>>))]
        [HttpGet]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult ChooseACentre()
        {
            // TODO: sort out ChooseACentre page
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            ChooseACentreViewModel model = TempData.Peek<ChooseACentreViewModel>();
            return View("ChooseACentre", model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<List<DelegateLoginDetails>>))]
        [HttpGet]
        public async Task<IActionResult> ChooseCentre(int centreId)
        {
            var rememberMe = (bool)TempData["RememberMe"];
            var userEntity = TempData.Peek<UserEntity>();
            var returnUrl = (string?)TempData["ReturnUrl"];
            TempData.Clear();

            // TODO: Likely need to filter this down further if we have admin/delegate account at this centre, but need to log in to only one because inactive etc.
            var userEntityWithJustTheSingleAccountsAttached = LoginHelper.FilterUserEntityForLoggingIntoSingleCentre(userEntity!, centreId);

            sessionService.StartAdminSession(userEntityWithJustTheSingleAccountsAttached!.AdminAccounts.SingleOrDefault()?.Id);

            return await LogIn(userEntityWithJustTheSingleAccountsAttached, rememberMe, returnUrl);
        }

        [HttpGet]
        public IActionResult AccountLocked(int failedCount)
        {
            return View(failedCount);
        }

        private void SetTempDataForChooseACentre(
            bool rememberMe,
            UserEntity userEntity,
            ChooseACentreViewModel chooseACentreViewModel,
            string? returnUrl
        )
        {
            TempData.Clear();
            TempData["RememberMe"] = rememberMe;
            TempData.Set(userEntity);
            TempData.Set(chooseACentreViewModel);
            TempData["ReturnUrl"] = returnUrl;
        }

        private async Task<IActionResult> LogIn(
            UserEntity userEntity,
            bool rememberMe,
            string? returnUrl
        )
        {
            var claims = LoginClaimsHelper.GetClaimsForSignIn(userEntity);
            var claimsIdentity = new ClaimsIdentity(claims, "Identity.Application");
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = rememberMe,
                IssuedUtc = DateTime.UtcNow
            };
            
            await HttpContext.SignInAsync("Identity.Application", new ClaimsPrincipal(claimsIdentity), authProperties);

            return RedirectToReturnUrl(returnUrl) ?? RedirectToAction("Index", "Home");
        }

        private IActionResult? RedirectToReturnUrl(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
            {
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                logger.LogWarning($"Attempted login redirect to non-local returnUrl {returnUrl}");
            }

            return null;
        }
    }
}
