namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Security.Claims;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using Microsoft.AspNetCore.Mvc;

    public class MyAccountController : Controller
    {
        private readonly ICentresService centresService;

        public MyAccountController(ICentresService centresService)
        {
            this.centresService = centresService;
        }

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Login");
            }

            var userEmail = User.GetCustomClaim(ClaimTypes.Email);
            var delegateNumber = User.GetCustomClaim(CustomClaimTypes.LearnCandidateNumber);
            var centreId = User.GetCentreId();
            var firstName = User.GetCustomClaim(CustomClaimTypes.UserForename);
            var surname = User.GetCustomClaim(CustomClaimTypes.UserSurname);
            var centreName = centresService.GetCentreName(centreId);

            var model = new MyAccountViewModel(centreName, userEmail, delegateNumber, firstName, surname);
            return View(model);
        }
    }
}
