namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem
{
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Policy = CustomPolicies.UserCentreAdminOnly)]
    [Route("/TrackingSystem/Delegates")]
    public class DelegatesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
