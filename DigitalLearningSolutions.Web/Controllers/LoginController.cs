namespace DigitalLearningSolutions.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.Login;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
    [SetSelectedTab(nameof(NavMenuTab.LogIn))]
    public class LoginController : Controller
    {
        private readonly IClockUtility clockUtility;
        private readonly IConfigDataService configDataService;
        private readonly ILogger<LoginController> logger;
        private readonly ILoginService loginService;
        private readonly ISessionService sessionService;
        private readonly IUserService userService;

        public LoginController(
            ILoginService loginService,
            ISessionService sessionService,
            ILogger<LoginController> logger,
            IUserService userService,
            IClockUtility clockUtility,
            IConfigDataService configDataService
        )
        {
            this.loginService = loginService;
            this.sessionService = sessionService;
            this.logger = logger;
            this.userService = userService;
            this.clockUtility = clockUtility;
            this.configDataService = configDataService;
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
                case LoginAttemptResult.InvalidCredentials:
                    ModelState.AddModelError("Password", "The credentials you have entered are incorrect");
                    return View("Index", model);
                case LoginAttemptResult.AccountsHaveMismatchedPasswords:
                    return View("MismatchingPasswords");
                case LoginAttemptResult.AccountLocked:
                    return View("AccountLocked");
                case LoginAttemptResult.InactiveAccount:
                    var supportEmail = configDataService.GetConfigValue(ConfigDataService.SupportEmail);
                    var inactiveAccountModel = new AccountInactiveViewModel(supportEmail!);
                    return View("AccountInactive", inactiveAccountModel);
                case LoginAttemptResult.UnverifiedEmail:
                    await this.CentrelessLogInAsync(loginResult.UserEntity!.UserAccount, model.RememberMe);
                    return RedirectToAction(
                        "Index",
                        "VerifyYourEmail",
                        new { emailVerificationReason = EmailVerificationReason.EmailNotVerified }
                    );
                case LoginAttemptResult.LogIntoSingleCentre:
                    return await LogIntoCentreAsync(
                        loginResult.UserEntity!,
                        model.RememberMe,
                        model.ReturnUrl,
                        loginResult.CentreToLogInto!.Value
                    );
                case LoginAttemptResult.ChooseACentre:
                    await this.CentrelessLogInAsync(loginResult.UserEntity!.UserAccount, model.RememberMe);
                    return RedirectToAction("ChooseACentre", "Login", new { returnUrl = model.ReturnUrl });
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [HttpGet]
        [Route("/{dlsSubApplication}/Login/ChooseACentre", Order = 1)]
        [Route("/Login/ChooseACentre", Order = 2)]
        [TypeFilter(typeof(ValidateAllowedDlsSubApplication))]
        [SetDlsSubApplication]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        [Authorize(Policy = CustomPolicies.BasicUser)]
        public IActionResult ChooseACentre(DlsSubApplication dlsSubApplication, string? returnUrl)
        {
            var userEntity = userService.GetUserById(User.GetUserId()!.Value);

            var (_, unverifiedCentreEmails) =
                userService.GetUnverifiedEmailsForUser(userEntity!.UserAccount.Id);
            var idsOfCentresWithUnverifiedEmails = unverifiedCentreEmails.Select(uce => uce.centreId).ToList();

            var chooseACentreAccountViewModels =
                loginService.GetChooseACentreAccountViewModels(userEntity, idsOfCentresWithUnverifiedEmails);

            var model = new ChooseACentreViewModel(
                chooseACentreAccountViewModels.OrderByDescending(account => account.IsActiveAdmin)
                    .ThenBy(account => account.CentreName).ToList(),
                returnUrl,
                userEntity.UserAccount.EmailVerified.HasValue,
                unverifiedCentreEmails.Count
            );

            return View("ChooseACentre", model);
        }

        [HttpPost]
        [Authorize(Policy = CustomPolicies.BasicUser)]
        [ServiceFilter(typeof(VerifyUserHasVerifiedPrimaryEmail))]
        public async Task<IActionResult> ChooseCentre(int centreId, string? returnUrl)
        {
            var userEntity = userService.GetUserById(User.GetUserIdKnownNotNull());
            var centreAccountSet = userEntity?.GetCentreAccountSet(centreId);

            if (centreAccountSet?.IsCentreActive != true)
            {
                return RedirectToAction("AccessDenied", "LearningSolutions");
            }

            var centreEmailIsUnverified = !loginService.CentreEmailIsVerified(userEntity.UserAccount.Id, centreId);

            if (centreEmailIsUnverified)
            {
                return RedirectToAction(
                    "Index",
                    "VerifyYourEmail",
                    new { emailVerificationReason = EmailVerificationReason.EmailNotVerified }
                );
            }

            var rememberMe = (await HttpContext.AuthenticateAsync()).Properties.IsPersistent;

            await HttpContext.Logout();

            return await LogIntoCentreAsync(userEntity!, rememberMe, returnUrl, centreId);
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
                IssuedUtc = clockUtility.UtcNow,
            };

            var adminAccount = userEntity!.GetCentreAccountSet(centreIdToLogInto)?.AdminAccount;

            if (adminAccount?.Active == true)
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
    }
}
