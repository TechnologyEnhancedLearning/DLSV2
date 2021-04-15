namespace DigitalLearningSolutions.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Login;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Mvc;

    public class LoginController : Controller
    {
        private readonly ILoginService loginService;
        private readonly IUserService userService;

        public LoginController(ILoginService loginService, IUserService userService)
        {
            this.loginService = loginService;
            this.userService = userService;
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

            var (unverifiedAdminUser, unverifiedDelegateUsers) = userService.GetUsersByUsername(model.Username);

            if (unverifiedAdminUser == null && unverifiedDelegateUsers.Count == 0)
            {
                ModelState.AddModelError("Username", "No account with that email address or user ID could be found.");
                return View("Index", model);
            }

            var (verifiedAdminUser, verifiedDelegateUsers) =
                loginService.VerifyUsers(model.Password, unverifiedAdminUser, unverifiedDelegateUsers);

            if (verifiedAdminUser == null && verifiedDelegateUsers.Count == 0)
            {
                ModelState.AddModelError("Password", "The password you have entered is incorrect.");
                return View("Index", model);
            }

            var approvedDelegateUser = verifiedDelegateUsers.FirstOrDefault(du => du.Approved);

            if (verifiedAdminUser == null && approvedDelegateUser == null)
            {
                return View("AccountNotApproved");
            }

            LogIn(verifiedAdminUser, approvedDelegateUser, model.Password, model.RememberMe);
            return RedirectToAction("Index", "Home");
        }

        private void LogIn(AdminUser? adminUser, DelegateUser? delegateUser, string password, bool rememberMe)
        {
            adminUser ??= loginService.GetVerifiedAdminUserAssociatedWithDelegateUser(delegateUser, password);

            var claims = GetClaimsForSignIn(adminUser, delegateUser);
            var claimsIdentity = new ClaimsIdentity(claims, "Identity.Application");
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = rememberMe,
                IssuedUtc = DateTime.UtcNow
            };
            HttpContext.SignInAsync("Identity.Application", new ClaimsPrincipal(claimsIdentity), authProperties);
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
