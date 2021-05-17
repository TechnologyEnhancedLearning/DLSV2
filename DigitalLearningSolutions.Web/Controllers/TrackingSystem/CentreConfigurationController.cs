namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Policy = CustomPolicies.UserCentreAdminOnly)]
    [Route("/TrackingSystem/CentreConfiguration")]
    public class CentreConfigurationController : Controller
    {
        private readonly ICentresDataService centresDataService;
        private readonly ICustomPromptsService customPromptsService;

        public CentreConfigurationController
        (
            ICentresDataService centresDataService,
            ICustomPromptsService customPromptsService
        )
        {
            this.centresDataService = centresDataService;
            this.customPromptsService = customPromptsService;
        }

        public IActionResult Index()
        {
            var centreId = User.GetCentreId();

            var centreDetails = centresDataService.GetCentreDetailsById(centreId);

            var model = new CentreConfigurationViewModel(centreDetails);

            return View("Index", model);
        }

        [Route("RegistrationPrompts")]
        public IActionResult RegistrationPrompts()
        {
            var centreId = User.GetCentreId();

            var customPrompts = customPromptsService.GetCustomPromptsForCentreByCentreId(centreId);

            var model = new DisplayRegistrationPromptsViewModel(customPrompts);

            return View(model);
        }

        [HttpGet]
        [Route("CentreManagerDetails/Edit")]
        public IActionResult EditCentreManagerDetails()
        {
            var centreId = User.GetCentreId();

            var centreDetails = centresDataService.GetCentreDetailsById(centreId);

            var model = new CentreManagerDetailsViewModel(centreDetails);

            return View("CentreManagerDetails", model);
        }

        [HttpPost]
        [Route("CentreManagerDetails/Edit")]
        public IActionResult EditCentreManagerDetails(CentreManagerDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("CentreManagerDetails", model);
            }

            centresDataService
                .UpdateCentreManagerDetails(model.CentreId, model.FirstName, model.LastName, model.Email, model.Telephone);

            return RedirectToAction("Index");
        }
    }
}
