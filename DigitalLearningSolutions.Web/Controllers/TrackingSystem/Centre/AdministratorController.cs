namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre
{
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Policy = CustomPolicies.UserCentreManagerOrUserUserAdminOnly)]
    [Route("TrackingSystem/Centre/Administrators")]
    public class AdministratorController : Controller
    {
        public IActionResult Index()
        {
            var centreId = User.GetCentreId();

            return View(centreId);
        }
    }
}
