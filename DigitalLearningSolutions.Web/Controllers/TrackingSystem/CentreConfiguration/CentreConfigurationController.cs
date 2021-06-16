namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.CentreConfiguration
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.ExternalApis;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CentreConfiguration;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("/TrackingSystem/CentreConfiguration")]
    public class CentreConfigurationController : Controller
    {
        private readonly ICentresDataService centresDataService;
        private readonly ILogger<CentreConfigurationController> logger;
        private readonly IMapsApiHelper mapsApiHelper;

        public CentreConfigurationController(
            ICentresDataService centresDataService,
            IMapsApiHelper mapsApiHelper,
            ILogger<CentreConfigurationController> logger
        )
        {
            this.centresDataService = centresDataService;
            this.mapsApiHelper = mapsApiHelper;
            this.logger = logger;
        }

        public IActionResult Index()
        {
            var centreId = User.GetCentreId();

            var centreDetails = centresDataService.GetCentreDetailsById(centreId)!;

            var model = new CentreConfigurationViewModel(centreDetails);

            return View("Index", model);
        }

        [HttpGet]
        [Route("EditCentreManagerDetails")]
        public IActionResult EditCentreManagerDetails()
        {
            var centreId = User.GetCentreId();

            var centreDetails = centresDataService.GetCentreDetailsById(centreId)!;

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
                .UpdateCentreManagerDetails(centreId, model.FirstName!, model.LastName!, model.Email!, model.Telephone);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("EditCentreWebsiteDetails")]
        public IActionResult EditCentreWebsiteDetails()
        {
            var centreId = User.GetCentreId();

            var centreDetails = centresDataService.GetCentreDetailsById(centreId)!;

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

            model.CentrePostcode = model.CentrePostcode!.Trim();
            var mapsResponse = mapsApiHelper.GeocodePostcode(model.CentrePostcode).Result;

            if (mapsResponse.HasNoResults())
            {
                ModelState.AddModelError(nameof(model.CentrePostcode), "Enter a valid postcode");
                return View(model);
            }

            if (mapsResponse.ApiErrorOccurred())
            {
                logger.LogWarning
                (
                    $"Failed Maps API call when trying to get postcode {model.CentrePostcode} " +
                    $"- status of {mapsResponse.Status} - error message: {mapsResponse.ErrorMessage}"
                );
                return RedirectToAction("Error", "LearningSolutions");
            }

            var longitude = double.Parse(mapsResponse.Results[0].Geometry.Location.Longitude);
            var latitude = double.Parse(mapsResponse.Results[0].Geometry.Location.Latitude);

            var centreId = User.GetCentreId();

            centresDataService.UpdateCentreWebsiteDetails(
                centreId,
                model.CentrePostcode,
                model.ShowCentreOnMap,
                longitude,
                latitude,
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
