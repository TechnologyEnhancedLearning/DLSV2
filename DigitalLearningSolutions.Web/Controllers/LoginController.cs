namespace DigitalLearningSolutions.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Login;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Mvc;

    public class LoginController : Controller
    {
        private readonly ILoginService loginService;

        public LoginController(ILoginService loginService)
        {
            this.loginService = loginService;
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

            AdminUser? adminUser;
            DelegateUser? delegateUser;

            try
            {
                (adminUser, delegateUser) = loginService.VerifyUserDetailsAndGetClaims(model.Username, model.Password);
            }
            catch (UserAccountNotFoundException e)
            {
                ModelState.AddModelError("Username", e.Message);
                return View("Index", model);
            }
            catch (IncorrectPasswordLoginException e)
            {
                ModelState.AddModelError("Password", e.Message);
                return View("Index", model);
            }
            catch (DelegateUserNotApprovedException)
            {
                return View("AccountNotApproved");
            }

            var claims = GetClaimsForSignIn(adminUser, delegateUser);
            var claimsIdentity = new ClaimsIdentity(claims, "Identity.Application");
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = model.RememberMe,
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
                new Claim(CustomClaimTypes.UserCentreId, adminUser?.CentreId.ToString() ?? delegateUser?.CentreId.ToString()),
                new Claim(CustomClaimTypes.UserCentreManager, adminUser?.IsCentreManager.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserCentreAdmin, adminUser?.CentreAdmin.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserUserAdmin, adminUser?.UserAdmin.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserContentCreator, adminUser?.ContentCreator.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserAuthenticatedCm, adminUser?.ContentManager.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserPublishToAll, adminUser?.PublishToAll.ToString() ?? "False"),
                new Claim(CustomClaimTypes.UserCentreReports, adminUser?.SummaryReports.ToString() ?? "False"),
                new Claim(CustomClaimTypes.LearnCandidateId, delegateUser?.Id.ToString() ?? "False"),
                new Claim(CustomClaimTypes.LearnUserAuthenticated, (delegateUser != null).ToString()),
                new Claim(CustomClaimTypes.AdminCategoryId, adminUser?.CategoryId.ToString() ?? "False"),
                new Claim(CustomClaimTypes.IsSupervisor, adminUser?.Supervisor.ToString() ?? "False"),
                new Claim(CustomClaimTypes.IsTrainer, adminUser?.Trainer.ToString() ?? "False"),
                new Claim(CustomClaimTypes.IsFrameworkDeveloper, adminUser?.IsFrameworkDeveloper.ToString() ?? "False")
            };

            var firstName = delegateUser?.FirstName ?? adminUser?.FirstName;
            var surname = delegateUser?.Surname ?? adminUser?.Surname;
            var centreName = delegateUser?.CentreName ?? adminUser?.CentreName;

            if (firstName != null)
            {
                claims.Add(new Claim(CustomClaimTypes.UserForename, firstName));
            }

            if (surname != null)
            {
                claims.Add(new Claim(CustomClaimTypes.UserSurname, surname));
            }

            if (centreName != null)
            {
                claims.Add(new Claim(CustomClaimTypes.UserCentreName, centreName));
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
