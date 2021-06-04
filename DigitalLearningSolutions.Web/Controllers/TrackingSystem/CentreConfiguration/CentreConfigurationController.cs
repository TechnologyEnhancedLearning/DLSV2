namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.CentreConfiguration
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CentreConfiguration;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
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

            var model = new CentreConfigurationViewModel(centreDetails);

            return View("Index", model);
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
                model.CentrePostcode!,
                model.ShowCentreOnMap,
                model.CentreTelephone,
                model.CentreEmail!,
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
