namespace DigitalLearningSolutions.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.Login;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    public class LoginController : Controller
    {
        private readonly ILoginService loginService;
        private readonly ISessionService sessionService;
        private readonly IUserService userService;
        private readonly ILogger<LoginController> logger;

        public LoginController(
            ILoginService loginService,
            IUserService userService,
            ISessionService sessionService,
            ILogger<LoginController> logger
        )
        {
            this.loginService = loginService;
            this.userService = userService;
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
        public IActionResult Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var (adminUser, delegateUsers) = userService.GetUsersByUsername(model.Username);
            if (adminUser == null && delegateUsers.Count == 0)
            {
                ModelState.AddModelError("Username", "No account with that email address or user ID could be found.");
                return View("Index", model);
            }

            var (verifiedAdminUser, verifiedDelegateUsers) =
                loginService.VerifyUsers(model.Password, adminUser, delegateUsers);
            if (verifiedAdminUser == null && verifiedDelegateUsers.Count == 0)
            {
                ModelState.AddModelError("Password", "The password you have entered is incorrect.");
                return View("Index", model);
            }

            var approvedDelegateUsers = verifiedDelegateUsers.Where(du => du.Approved).ToList();
            if (verifiedAdminUser == null && !approvedDelegateUsers.Any())
            {
                return View("AccountNotApproved");
            }

            verifiedAdminUser ??=
                loginService.GetVerifiedAdminUserAssociatedWithDelegateUser(
                    verifiedDelegateUsers.First(),
                    model.Password
                );

            var (verifiedAdminUserWithActiveCentre, approvedDelegateUsersWithActiveCentre) =
                userService.GetUsersWithActiveCentres(verifiedAdminUser, approvedDelegateUsers);
            var availableCentres =
                userService.GetUserCentres(verifiedAdminUserWithActiveCentre, approvedDelegateUsersWithActiveCentre);

            if (availableCentres.Count == 0)
            {
                return View("CentreInactive");
            }

            var (adminLoginDetails, delegateLoginDetails) =
                GetLoginDetails(verifiedAdminUserWithActiveCentre, approvedDelegateUsersWithActiveCentre);

            if (availableCentres.Count == 1)
            {
                sessionService.StartAdminSession(adminLoginDetails?.Id);

                return LogIn(
                    adminLoginDetails,
                    delegateLoginDetails.FirstOrDefault(),
                    model.RememberMe,
                    model.ReturnUrl
                );
            }

            var chooseACentreViewModel = new ChooseACentreViewModel(availableCentres);

            SetTempDataForChooseACentre(
                model.RememberMe,
                adminLoginDetails,
                delegateLoginDetails,
                chooseACentreViewModel,
                model.ReturnUrl
            );

            return RedirectToAction("ChooseACentre", "Login");
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
        public IActionResult ChooseCentre(int centreId)
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
            return LogIn(adminAccountForChosenCentre, delegateAccountForChosenCentre, rememberMe, returnUrl);
        }

        private (AdminLoginDetails?, List<DelegateLoginDetails>) GetLoginDetails(
            AdminUser? adminUser,
            List<DelegateUser> delegateUsers
        )
        {
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

        private IActionResult LogIn(
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
            HttpContext.SignInAsync("Identity.Application", new ClaimsPrincipal(claimsIdentity), authProperties);

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
