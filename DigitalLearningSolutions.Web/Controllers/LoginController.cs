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
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.Login;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Mvc;

    public class LoginController : Controller
    {
        private readonly ILoginService loginService;
        private readonly ISessionService sessionService;
        private readonly IUserService userService;

        public LoginController(ILoginService loginService, IUserService userService, ISessionService sessionService)
        {
            this.loginService = loginService;
            this.userService = userService;
            this.sessionService = sessionService;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
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
                loginService.GetVerifiedAdminUserAssociatedWithDelegateUser(verifiedDelegateUsers.First(),
                    model.Password);

            var availableCentres = userService.GetUserCentres(verifiedAdminUser, approvedDelegateUsers);
            if (availableCentres.Count == 1)
            {
                if (verifiedAdminUser != null)
                {
                    sessionService.StartAdminSession(verifiedAdminUser.Id);
                }

                return LogIn(verifiedAdminUser, approvedDelegateUsers.FirstOrDefault(), model.RememberMe);
            }

            ChooseACentreViewModel chooseACentreViewModel = new ChooseACentreViewModel(availableCentres);
            SetTempDataForChooseACentre
            (
                model.RememberMe,
                verifiedAdminUser,
                approvedDelegateUsers,
                chooseACentreViewModel
            );

            return RedirectToAction("ChooseACentre", "Login");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<ChooseACentreViewModel>))]
        [HttpGet]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult ChooseACentre()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = TempData.Peek<ChooseACentreViewModel>();

            return View("ChooseACentre", model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<List<DelegateUser>>))]
        [HttpGet]
        public IActionResult ChooseCentre(int centreId)
        {
            var rememberMe = (bool)TempData["RememberMe"];
            var adminAccount = TempData.Peek<AdminUser>();
            var approvedDelegateAccounts = TempData.Peek<List<DelegateUser>>();
            TempData.Clear();

            var adminAccountForChosenCentre = adminAccount?.CentreId == centreId ? adminAccount : null;
            var delegateAccountForChosenCentre =
                approvedDelegateAccounts?.FirstOrDefault(du => du.CentreId == centreId);
            if (adminAccountForChosenCentre != null)
            {
                sessionService.StartAdminSession(adminAccountForChosenCentre.Id);
            }

            return LogIn(adminAccountForChosenCentre, delegateAccountForChosenCentre, rememberMe);
        }

        private void SetTempDataForChooseACentre(bool rememberMe, AdminUser? adminUser,
            List<DelegateUser> approvedDelegateUsers, ChooseACentreViewModel chooseACentreViewModel)
        {
            if (adminUser != null)
            {
                adminUser.ProfileImage = null;
            }

            foreach (var user in approvedDelegateUsers.Where(du => du.ProfileImage != null))
            {
                user.ProfileImage = null;
            }

            TempData.Clear();
            TempData["RememberMe"] = rememberMe;
            TempData.Set(adminUser);
            TempData.Set(approvedDelegateUsers);
            TempData.Set(chooseACentreViewModel);
        }

        private IActionResult LogIn(AdminUser? adminUser, DelegateUser? delegateUser, bool rememberMe)
        {
            var claims = GetClaimsForSignIn(adminUser, delegateUser);
            var claimsIdentity = new ClaimsIdentity(claims, "Identity.Application");
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = rememberMe,
                IssuedUtc = DateTime.UtcNow
            };
            HttpContext.SignInAsync("Identity.Application", new ClaimsPrincipal(claimsIdentity), authProperties);
            return RedirectToAction("Index", "Home");
        }

        private List<Claim> GetClaimsForSignIn(AdminUser? adminUser, DelegateUser? delegateUser)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, adminUser?.EmailAddress ?? delegateUser?.EmailAddress),
                new Claim(CustomClaimTypes.UserCentreId,
                    adminUser?.CentreId.ToString() ?? delegateUser?.CentreId.ToString()),
                new Claim(CustomClaimTypes.UserCentreManager, adminUser?.IsCentreManager.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserCentreAdmin, adminUser?.IsCentreAdmin.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserUserAdmin, adminUser?.IsUserAdmin.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserContentCreator, adminUser?.IsContentCreator.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserAuthenticatedCm, adminUser?.IsContentManager.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserPublishToAll, adminUser?.PublishToAll.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserCentreReports, adminUser?.SummaryReports.ToString() ?? "False"),
                new Claim(CustomClaimTypes.LearnCandidateId, delegateUser?.Id.ToString() ?? "False"),
                new Claim(CustomClaimTypes.LearnUserAuthenticated, (delegateUser != null).ToString()),
                new Claim(CustomClaimTypes.AdminCategoryId, adminUser?.CategoryId.ToString() ?? "False"),
                new Claim(CustomClaimTypes.IsSupervisor, adminUser?.IsSupervisor.ToString() ?? "False"),
                new Claim(CustomClaimTypes.IsTrainer, adminUser?.IsTrainer.ToString() ?? "False"),
                new Claim(CustomClaimTypes.IsFrameworkDeveloper, adminUser?.IsFrameworkDeveloper.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserCentreName, adminUser?.CentreName ?? delegateUser?.CentreName)
            };

            var firstName = adminUser?.FirstName ?? delegateUser?.FirstName;
            var surname = adminUser?.LastName ?? delegateUser?.LastName;

            if (firstName != null)
            {
                claims.Add(new Claim(CustomClaimTypes.UserForename, firstName));
            }

            if (surname != null)
            {
                claims.Add(new Claim(CustomClaimTypes.UserSurname, surname));
            }

            if (delegateUser?.CandidateNumber != null)
            {
                claims.Add(new Claim(CustomClaimTypes.LearnCandidateNumber, delegateUser.CandidateNumber));
            }

            if (adminUser?.Id != null)
            {
                claims.Add(new Claim(CustomClaimTypes.UserAdminId, adminUser.Id.ToString()));
            }

            return claims;
        }
    }
}
