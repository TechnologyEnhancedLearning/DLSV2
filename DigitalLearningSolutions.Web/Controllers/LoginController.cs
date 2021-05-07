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

            var verifiedAdminUserWithActiveCentre = verifiedAdminUser?.CentreActive == true ? verifiedAdminUser : null;
            var approvedDelegateUsersWithActiveCentre = approvedDelegateUsers.Where(du => du.CentreActive).ToList();

            var availableCentres = userService.GetUserCentres(verifiedAdminUserWithActiveCentre, approvedDelegateUsersWithActiveCentre);

            if (availableCentres.Count == 0)
            {
                return View("CentreInactive");
            }
            
            var (adminLoginDetails, delegateLoginDetails) =
                GetLoginDetails(verifiedAdminUserWithActiveCentre, approvedDelegateUsersWithActiveCentre);
                
            if (availableCentres.Count == 1)
            {
                sessionService.StartAdminSession(adminLoginDetails?.Id);

                return LogIn(adminLoginDetails, delegateLoginDetails.FirstOrDefault(), model.RememberMe);
            }

            SetTempDataForChooseACentre
            (
                model.RememberMe,
                adminLoginDetails,
                delegateLoginDetails,
                availableCentres
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

            var availableCentres = TempData.Peek<List<CentreUserDetails>>();
            ChooseACentreViewModel model = new ChooseACentreViewModel(availableCentres);
            return View("ChooseACentre", model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<List<DelegateLoginDetails>>))]
        [HttpGet]
        public IActionResult ChooseCentre(int centreId)
        {
            var rememberMe = (bool)TempData["RememberMe"];
            var adminLoginDetails = TempData.Peek<AdminLoginDetails>();
            var delegateLoginDetails = TempData.Peek<List<DelegateLoginDetails>>();
            TempData.Clear();

            var adminAccountForChosenCentre = adminLoginDetails?.CentreId == centreId ? adminLoginDetails : null;
            var delegateAccountForChosenCentre =
                delegateLoginDetails?.FirstOrDefault(du => du.CentreId == centreId);

            sessionService.StartAdminSession(adminAccountForChosenCentre?.Id);
            return LogIn(adminAccountForChosenCentre, delegateAccountForChosenCentre, rememberMe);
        }

        private (AdminLoginDetails?, List<DelegateLoginDetails>) GetLoginDetails
        (
            AdminUser? adminUser,
            List<DelegateUser> delegateUsers
        )
        {
            var adminLoginDetails = adminUser != null ? new AdminLoginDetails(adminUser) : null;
            var delegateLoginDetails = delegateUsers.Select(du => new DelegateLoginDetails(du)).ToList();
            return (adminLoginDetails, delegateLoginDetails);
        }

        private void SetTempDataForChooseACentre
        (
            bool rememberMe,
            AdminLoginDetails? adminLoginDetails,
            List<DelegateLoginDetails> delegateLoginDetails,
            List<CentreUserDetails> availableCentres
        )
        {
            TempData.Clear();
            TempData["RememberMe"] = rememberMe;
            TempData.Set(adminLoginDetails);
            TempData.Set(delegateLoginDetails);
            TempData.Set(availableCentres);
        }

        private IActionResult LogIn
        (
            AdminLoginDetails? adminLoginDetails,
            DelegateLoginDetails? delegateLoginDetails,
            bool rememberMe
        )
        {
            var claims = GetClaimsForSignIn(adminLoginDetails, delegateLoginDetails);
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

        private List<Claim> GetClaimsForSignIn
        (
            AdminLoginDetails? adminLoginDetails,
            DelegateLoginDetails? delegateLoginDetails
        )
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, adminLoginDetails?.EmailAddress ?? delegateLoginDetails?.EmailAddress),
                new Claim(CustomClaimTypes.UserCentreId,
                    adminLoginDetails?.CentreId.ToString() ?? delegateLoginDetails?.CentreId.ToString()),
                new Claim(CustomClaimTypes.UserCentreManager, adminLoginDetails?.IsCentreManager.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserCentreAdmin, adminLoginDetails?.IsCentreAdmin.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserUserAdmin, adminLoginDetails?.IsUserAdmin.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserContentCreator,
                    adminLoginDetails?.IsContentCreator.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserAuthenticatedCm,
                    adminLoginDetails?.IsContentManager.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserPublishToAll, adminLoginDetails?.PublishToAll.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserCentreReports, adminLoginDetails?.SummaryReports.ToString() ?? "False"),
                new Claim(CustomClaimTypes.LearnCandidateId, delegateLoginDetails?.Id.ToString() ?? "False"),
                new Claim(CustomClaimTypes.LearnUserAuthenticated, (delegateLoginDetails != null).ToString()),
                new Claim(CustomClaimTypes.AdminCategoryId, adminLoginDetails?.CategoryId.ToString() ?? "False"),
                new Claim(CustomClaimTypes.IsSupervisor, adminLoginDetails?.IsSupervisor.ToString() ?? "False"),
                new Claim(CustomClaimTypes.IsTrainer, adminLoginDetails?.IsTrainer.ToString() ?? "False"),
                new Claim(CustomClaimTypes.IsFrameworkDeveloper,
                    adminLoginDetails?.IsFrameworkDeveloper.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserCentreName,
                    adminLoginDetails?.CentreName ?? delegateLoginDetails?.CentreName)
            };

            var firstName = adminLoginDetails?.FirstName ?? delegateLoginDetails?.FirstName;
            var surname = adminLoginDetails?.LastName ?? delegateLoginDetails?.LastName;

            if (firstName != null)
            {
                claims.Add(new Claim(CustomClaimTypes.UserForename, firstName));
            }

            if (surname != null)
            {
                claims.Add(new Claim(CustomClaimTypes.UserSurname, surname));
            }

            if (delegateLoginDetails?.CandidateNumber != null)
            {
                claims.Add(new Claim(CustomClaimTypes.LearnCandidateNumber, delegateLoginDetails.CandidateNumber));
            }

            if (adminLoginDetails?.Id != null)
            {
                claims.Add(new Claim(CustomClaimTypes.UserAdminId, adminLoginDetails.Id.ToString()));
            }

            return claims;
        }
    }
}
