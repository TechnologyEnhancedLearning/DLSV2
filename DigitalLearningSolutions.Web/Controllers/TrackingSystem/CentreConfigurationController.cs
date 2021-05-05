namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Policy = CustomPolicies.UserCentreAdminOnly)]
    [Route("/TrackingSystem/CentreConfiguration")]
    public class CentreConfigurationController : Controller
    {
        private readonly ICentresDataService centresDataService;

        public CentreConfigurationController(ICentresDataService centresDataService)
        {
            this.centresDataService = centresDataService;
        }

        public IActionResult Index()
        {
            var centreId = User.GetCentreId();

            var centreDetails = centresDataService.GetCentreDetailsById(centreId);

            var model = new CentreConfigurationViewModel(centreDetails.CentreName, centreDetails.RegionName);

            return View("~/Views/TrackingSystem/CentreConfiguration/Index.cshtml", model);
        }
    }
}
