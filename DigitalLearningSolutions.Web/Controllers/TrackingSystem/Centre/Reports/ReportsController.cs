namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Reports
{
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("/TrackingSystem/Centre/Reports")]
    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            var model = new ReportsViewModel();
            return View(model);
        }
    }
}
