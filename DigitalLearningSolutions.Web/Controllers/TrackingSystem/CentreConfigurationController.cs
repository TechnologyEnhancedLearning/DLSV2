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
        [Route("EditCentreManagerDetails")]
        public IActionResult EditCentreManagerDetails()
        {
            var centreId = User.GetCentreId();

            var centreDetails = centresDataService.GetCentreDetailsById(centreId);

            var model = new EditCentreManagerDetailsViewModel(centreDetails);

            return View(model);
        }

        [HttpPost]
        [Route("EditCentreManagerDetails")]
        public IActionResult EditCentreManagerDetails(EditCentreManagerDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var centreId = User.GetCentreId();

            centresDataService
                .UpdateCentreManagerDetails(centreId, model.FirstName, model.LastName, model.Email, model.Telephone);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("EditCentreWebsiteDetails")]
        public IActionResult EditCentreWebsiteDetails()
        {
            var centreId = User.GetCentreId();

            var centreDetails = centresDataService.GetCentreDetailsById(centreId);

            var model = new EditCentreWebsiteDetailsViewModel(centreDetails);

            return View(model);
        }

        [HttpPost]
        [Route("EditCentreWebsiteDetails")]
        public IActionResult EditCentreWebsiteDetails(EditCentreWebsiteDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var centreId = User.GetCentreId();

            centresDataService.UpdateCentreWebsiteDetails
            (
                centreId,
                model.CentrePostcode,
                model.CentreTelephone,
                model.CentreEmail,
                model.OpeningHours,
                model.CentreWebAddress,
                model.OrganisationsCovered,
                model.TrainingVenues,
                model.OtherInformation
            );

            return RedirectToAction("Index");
        }
    }
}
