namespace DigitalLearningSolutions.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Login;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.Login)]
    public class LoginController : Controller
    {
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
            // TODO: HEEDLS-364 - Overwrite this old code for automatic sign in with new code that signs in the user after validation
            if (model.Username?.ToLower() == "testusernameerror")
            {
                ModelState.AddModelError("username", "There is a username error.");
            }

            if (model.Username?.ToLower() == "testpassworderror")
            {
                ModelState.AddModelError("password", "There is a password error.");
            }

            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, "kevin.whittaker1@nhs.net"),
                new Claim(CustomClaimTypes.UserCentreId, "101"),
                new Claim(CustomClaimTypes.UserCentreManager, "True"),
                new Claim(CustomClaimTypes.LearnCandidateId, "254480"),
                new Claim(CustomClaimTypes.UserCentreAdmin, "True"),
                new Claim(CustomClaimTypes.UserUserAdmin, "True"),
                new Claim(CustomClaimTypes.UserContentCreator, "True"),
                new Claim(CustomClaimTypes.UserAuthenticatedCm, "True"),
                new Claim(CustomClaimTypes.UserPublishToAll, "True"),
                new Claim(CustomClaimTypes.UserCentreReports, "True"),
                new Claim(CustomClaimTypes.LearnUserAuthenticated, "True"),
                new Claim(CustomClaimTypes.AdminCategoryId, "0"),
                new Claim(CustomClaimTypes.IsSupervisor, "True"),
                new Claim(CustomClaimTypes.IsTrainer, "False"),
                new Claim(CustomClaimTypes.IsFrameworkDeveloper, "True"),
                new Claim(CustomClaimTypes.LearnCandidateNumber, "KW1205"),
                new Claim(CustomClaimTypes.UserForename, "Kevin"),
                new Claim(CustomClaimTypes.UserSurname, "Whittaker(Developer)"),
                new Claim(CustomClaimTypes.UserCentreName, "Test Centre NHSD"),
                new Claim(CustomClaimTypes.UserAdminId, "1")
            };
            var claimsIdentity = new ClaimsIdentity(claims, "Identity.Application");
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = true,
                IssuedUtc = DateTime.UtcNow
            };
            HttpContext.SignInAsync("Identity.Application", new ClaimsPrincipal(claimsIdentity), authProperties);
            return RedirectToAction("Index", "Home");
        }
    }
}
