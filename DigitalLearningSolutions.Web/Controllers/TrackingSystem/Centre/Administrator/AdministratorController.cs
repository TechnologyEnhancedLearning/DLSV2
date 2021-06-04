namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Administrator
{
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Policy = CustomPolicies.UserCentreManager)]
    [Route("TrackingSystem/Centre/Administrators")]
    public class AdministratorController : Controller
    {
        public IActionResult Index()
        {
            var model = new CentreAdministratorViewModel(User.GetCentreId());

            return View(model);
        }
    }
}
