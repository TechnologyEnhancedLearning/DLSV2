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
        private readonly IUserService userService;
        private readonly ICryptoService cryptoService;

        public LoginController(IUserService userService, ICryptoService cryptoService)
        {
            this.userService = userService;
            this.cryptoService = cryptoService;
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

            AdminUser? adminUser = userService.GetAdminUserByUsername(model.Username);
            List<DelegateUser> delegateUsers = userService.GetDelegateUsersByUsername(model.Username);

            if (adminUser == null && delegateUsers.Count == 0)
            {
                ModelState.AddModelError("Username","No account with that email address or user ID could be found.");
                return View("Index", model);
            }

            adminUser = cryptoService.VerifyHashedPassword(adminUser?.Password, model.Password) ? adminUser : null;
            delegateUsers = delegateUsers.Where(du => cryptoService.VerifyHashedPassword(du.Password, model.Password)).ToList();

            if (adminUser == null && delegateUsers.Count == 0)
            {
                ModelState.AddModelError("Password", "The password you have entered is incorrect.");
                return View("Index", model);
            }

            if (adminUser == null)
            {
                foreach (var delegateUser in delegateUsers.Where(du => du.EmailAddress != null))
                {
                    adminUser = userService.GetAdminUserByUsername(delegateUser.EmailAddress);

                    if (cryptoService.VerifyHashedPassword(adminUser?.Password, model.Password))
                    {
                        break;
                    }

                    adminUser = null;
                }

                if (adminUser == null && delegateUsers.Any(du => !du.Approved) )
                {
                    return View("AccountNotApproved");
                }
            }

            var claims = GetClaimsForSignIn(adminUser, delegateUsers.FirstOrDefault(du => du.Approved));
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

            var firstName = delegateUser?.FirstName ?? adminUser?.FirstName;
            var surname = delegateUser?.Surname ?? adminUser?.Surname;

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
