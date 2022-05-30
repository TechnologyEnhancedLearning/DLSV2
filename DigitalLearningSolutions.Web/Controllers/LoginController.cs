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
            var (adminLoginDetails, delegateLoginDetails) = GetLoginDetails(loginResult.Accounts);
            switch (loginResult.LoginAttemptResult)
            {
                case LoginAttemptResult.InvalidUsername:
                    ModelState.AddModelError(
                        "Username",
                        "A user with this email or user ID could not be found"
                    );
                    return View("Index", model);
                case LoginAttemptResult.InvalidPassword:
                    ModelState.AddModelError("Password", "The password you have entered is incorrect");
                    return View("Index", model);
                case LoginAttemptResult.AccountLocked:
                    return RedirectToAction(
                        "AccountLocked",
                        new { failedCount = loginResult.Accounts.AdminAccount!.FailedLoginCount }
                    );
                case LoginAttemptResult.AccountNotApproved:
                    return View("AccountNotApproved");
                case LoginAttemptResult.InactiveCentre:
                    return View("CentreInactive");
                case LoginAttemptResult.LogIntoSingleCentre:
                    sessionService.StartAdminSession(adminLoginDetails?.Id);
                    return await LogIn(
                        adminLoginDetails,
                        delegateLoginDetails.FirstOrDefault(),
                        model.RememberMe,
                        model.ReturnUrl
                    );
                case LoginAttemptResult.ChooseACentre:
                    var chooseACentreViewModel = new ChooseACentreViewModel(loginResult.AvailableCentres);
                    SetTempDataForChooseACentre(
                        model.RememberMe,
                        adminLoginDetails,
                        delegateLoginDetails,
                        chooseACentreViewModel,
                        model.ReturnUrl
                    );
                    return RedirectToAction("ChooseACentre", "Login");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<List<CentreUserDetails>>))]
        [HttpGet]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult ChooseACentre()
        {
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
            var adminLoginDetails = TempData.Peek<AdminLoginDetails>();
            var delegateLoginDetails = TempData.Peek<List<DelegateLoginDetails>>();
            var returnUrl = (string?)TempData["ReturnUrl"];
            TempData.Clear();

            var adminAccountForChosenCentre = adminLoginDetails?.CentreId == centreId ? adminLoginDetails : null;
            var delegateAccountForChosenCentre =
                delegateLoginDetails?.FirstOrDefault(du => du.CentreId == centreId);

            sessionService.StartAdminSession(adminAccountForChosenCentre?.Id);
            return await LogIn(adminAccountForChosenCentre, delegateAccountForChosenCentre, rememberMe, returnUrl);
        }

        [HttpGet]
        public IActionResult AccountLocked(int failedCount)
        {
            return View(failedCount);
        }

        private (AdminLoginDetails?, List<DelegateLoginDetails>) GetLoginDetails(
            UserAccountSet accounts
        )
        {
            var (adminUser, delegateUsers) = accounts;
            var adminLoginDetails = adminUser != null ? new AdminLoginDetails(adminUser) : null;
            var delegateLoginDetails = delegateUsers.Select(du => new DelegateLoginDetails(du)).ToList();
            return (adminLoginDetails, delegateLoginDetails);
        }

        private void SetTempDataForChooseACentre(
            bool rememberMe,
            AdminLoginDetails? adminLoginDetails,
            List<DelegateLoginDetails> delegateLoginDetails,
            ChooseACentreViewModel chooseACentreViewModel,
            string? returnUrl
        )
        {
            TempData.Clear();
            TempData["RememberMe"] = rememberMe;
            TempData.Set(adminLoginDetails);
            TempData.Set(delegateLoginDetails);
            TempData.Set(chooseACentreViewModel);
            TempData["ReturnUrl"] = returnUrl;
        }

        private async Task<IActionResult> LogIn(
            AdminLoginDetails? adminLoginDetails,
            DelegateLoginDetails? delegateLoginDetails,
            bool rememberMe,
            string? returnUrl
        )
        {
            var claims = LoginClaimsHelper.GetClaimsForSignIn(adminLoginDetails, delegateLoginDetails);
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
