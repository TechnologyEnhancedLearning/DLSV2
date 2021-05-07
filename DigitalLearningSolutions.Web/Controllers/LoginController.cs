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
            (AdminClaimDetails? adminClaimDetails, List<DelegateClaimDetails> delegateClaimDetails) =
                GetClaimsDetails(verifiedAdminUser, approvedDelegateUsers);
            if (availableCentres.Count == 1)
            {
                if (adminClaimDetails != null)
                {
                    sessionService.StartAdminSession(adminClaimDetails.Id);
                }

                return LogIn(adminClaimDetails, delegateClaimDetails.FirstOrDefault(), model.RememberMe);
            }

            ChooseACentreViewModel chooseACentreViewModel = new ChooseACentreViewModel(availableCentres);
            SetTempDataForChooseACentre
            (
                model.RememberMe,
                adminClaimDetails,
                delegateClaimDetails,
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
            var adminClaimDetails = TempData.Peek<AdminClaimDetails>();
            var delegateClaimDetails = TempData.Peek<List<DelegateClaimDetails>>();
            TempData.Clear();

            var adminAccountForChosenCentre = adminClaimDetails?.CentreId == centreId ? adminClaimDetails : null;
            var delegateAccountForChosenCentre =
                delegateClaimDetails?.FirstOrDefault(du => du.CentreId == centreId);
            if (adminAccountForChosenCentre != null)
            {
                sessionService.StartAdminSession(adminAccountForChosenCentre.Id);
            }

            return LogIn(adminAccountForChosenCentre, delegateAccountForChosenCentre, rememberMe);
        }

        private (AdminClaimDetails?, List<DelegateClaimDetails>) GetClaimsDetails(AdminUser? adminUser, List<DelegateUser> delegateUsers)
        {
            var adminClaimDetails = adminUser != null ? new AdminClaimDetails(adminUser) : null;
            var delegateClaimDetails = delegateUsers.Select(du => new DelegateClaimDetails(du)).ToList();
            return (adminClaimDetails, delegateClaimDetails);
        }

        private void SetTempDataForChooseACentre(bool rememberMe, AdminClaimDetails? adminClaimDetails,
            List<DelegateClaimDetails> delegateClaimDetails, ChooseACentreViewModel chooseACentreViewModel)
        {
            TempData.Clear();
            TempData["RememberMe"] = rememberMe;
            TempData.Set(adminClaimDetails);
            TempData.Set(delegateClaimDetails);
            TempData.Set(chooseACentreViewModel);
        }

        private IActionResult LogIn(AdminClaimDetails? adminClaimDetails, DelegateClaimDetails? delegateClaimDetails, bool rememberMe)
        {
            var claims = GetClaimsForSignIn(adminClaimDetails, delegateClaimDetails);
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

        private List<Claim> GetClaimsForSignIn(AdminClaimDetails? adminClaimDetails, DelegateClaimDetails? delegateClaimDetails)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, adminClaimDetails?.EmailAddress ?? delegateClaimDetails?.EmailAddress),
                new Claim(CustomClaimTypes.UserCentreId,
                    adminClaimDetails?.CentreId.ToString() ?? delegateClaimDetails?.CentreId.ToString()),
                new Claim(CustomClaimTypes.UserCentreManager, adminClaimDetails?.IsCentreManager.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserCentreAdmin, adminClaimDetails?.IsCentreAdmin.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserUserAdmin, adminClaimDetails?.IsUserAdmin.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserContentCreator, adminClaimDetails?.IsContentCreator.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserAuthenticatedCm, adminClaimDetails?.IsContentManager.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserPublishToAll, adminClaimDetails?.PublishToAll.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserCentreReports, adminClaimDetails?.SummaryReports.ToString() ?? "False"),
                new Claim(CustomClaimTypes.LearnCandidateId, delegateClaimDetails?.Id.ToString() ?? "False"),
                new Claim(CustomClaimTypes.LearnUserAuthenticated, (delegateClaimDetails != null).ToString()),
                new Claim(CustomClaimTypes.AdminCategoryId, adminClaimDetails?.CategoryId.ToString() ?? "False"),
                new Claim(CustomClaimTypes.IsSupervisor, adminClaimDetails?.IsSupervisor.ToString() ?? "False"),
                new Claim(CustomClaimTypes.IsTrainer, adminClaimDetails?.IsTrainer.ToString() ?? "False"),
                new Claim(CustomClaimTypes.IsFrameworkDeveloper, adminClaimDetails?.IsFrameworkDeveloper.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserCentreName, adminClaimDetails?.CentreName ?? delegateClaimDetails?.CentreName)
            };

            var firstName = adminClaimDetails?.FirstName ?? delegateClaimDetails?.FirstName;
            var surname = adminClaimDetails?.LastName ?? delegateClaimDetails?.LastName;

            if (firstName != null)
            {
                claims.Add(new Claim(CustomClaimTypes.UserForename, firstName));
            }

            if (surname != null)
            {
                claims.Add(new Claim(CustomClaimTypes.UserSurname, surname));
            }

            if (delegateClaimDetails?.CandidateNumber != null)
            {
                claims.Add(new Claim(CustomClaimTypes.LearnCandidateNumber, delegateClaimDetails.CandidateNumber));
            }

            if (adminClaimDetails?.Id != null)
            {
                claims.Add(new Claim(CustomClaimTypes.UserAdminId, adminClaimDetails.Id.ToString()));
            }

            return claims;
        }
    }
}
