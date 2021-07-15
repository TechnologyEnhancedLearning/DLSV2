namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.CentreConfiguration
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.ExternalApis;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CentreConfiguration;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("/TrackingSystem/CentreConfiguration")]
    public class CentreConfigurationController : Controller
    {
        private readonly ICentresDataService centresDataService;
        private readonly ILogger<CentreConfigurationController> logger;
        private readonly IMapsApiHelper mapsApiHelper;
        private readonly IImageResizeService imageResizeService;

        public CentreConfigurationController(
            ICentresDataService centresDataService,
            IMapsApiHelper mapsApiHelper,
            ILogger<CentreConfigurationController> logger,
            IImageResizeService imageResizeService
        )
        {
            this.centresDataService = centresDataService;
            this.mapsApiHelper = mapsApiHelper;
            this.logger = logger;
            this.imageResizeService = imageResizeService;
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
                ModelState.AddModelError(nameof(model.CentrePostcode), "Enter a UK postcode");
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

            var latitude = double.Parse(mapsResponse.Results[0].Geometry.Location.Latitude);
            var longitude = double.Parse(mapsResponse.Results[0].Geometry.Location.Longitude);

            var centreId = User.GetCentreId();

            centresDataService.UpdateCentreWebsiteDetails(
                centreId,
                model.CentrePostcode,
                model.ShowCentreOnMap,
                latitude,
                longitude,
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

        [HttpGet]
        [Route("EditCentreDetails")]
        public IActionResult EditCentreDetails()
        {
            var centreId = User.GetCentreId();

            var centreDetails = centresDataService.GetCentreDetailsById(centreId)!;

            var model = new EditCentreDetailsViewModel(centreDetails);

            return View(model);
        }

        [HttpPost]
        [Route("EditCentreDetails")]
        public IActionResult EditCentreDetails(EditCentreDetailsViewModel model, string action)
        {
            return action switch
            {
                "save" => EditCentreDetailsPostSave(model),
                "previewSignature" => EditCentreDetailsPostPreviewSignature(model),
                "removeSignature" => EditCentreDetailsPostRemoveSignature(model),
                "previewLogo" => EditCentreDetailsPostPreviewLogo(model),
                "removeLogo" => EditCentreDetailsPostRemoveLogo(model),
                _ => new StatusCodeResult(500)
            };
        }

        private IActionResult EditCentreDetailsPostSave(EditCentreDetailsViewModel model)
        {
            if (model.CentreSignatureFile != null)
            {
                ModelState.AddModelError(nameof(EditCentreDetailsViewModel.CentreSignatureFile),
                    "Preview your new centre signature before saving");
            }

            if (model.CentreLogoFile != null)
            {
                ModelState.AddModelError(nameof(EditCentreDetailsViewModel.CentreLogoFile),
                    "Preview your new centre logo before saving");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var centreId = User.GetCentreId();

            centresDataService.UpdateCentreDetails(
                centreId,
                model.NotifyEmail,
                model.BannerText!,
                model.CentreSignature,
                model.CentreLogo
            );

            return RedirectToAction("Index");
        }

        private IActionResult EditCentreDetailsPostPreviewSignature(EditCentreDetailsViewModel model)
        {
            ModelState.ClearErrorsForAllFieldsExcept(nameof(EditCentreDetailsViewModel.CentreSignatureFile));

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.CentreSignatureFile != null)
            {
                ModelState.Remove(nameof(EditCentreDetailsViewModel.CentreSignature));
                model.CentreSignature = imageResizeService.ResizeCentreImage(model.CentreSignatureFile);
            }

            return View(model);
        }

        private IActionResult EditCentreDetailsPostRemoveSignature(EditCentreDetailsViewModel model)
        {
            ModelState.ClearAllErrors();

            ModelState.Remove(nameof(EditCentreDetailsViewModel.CentreSignature));
            model.CentreSignature = null;
            return View(model);
        }

        private IActionResult EditCentreDetailsPostPreviewLogo(EditCentreDetailsViewModel model)
        {
            ModelState.ClearErrorsForAllFieldsExcept(nameof(EditCentreDetailsViewModel.CentreLogoFile));

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.CentreLogoFile != null)
            {
                ModelState.Remove(nameof(EditCentreDetailsViewModel.CentreLogo));
                model.CentreLogo = imageResizeService.ResizeCentreImage(model.CentreLogoFile);
            }

            return View(model);
        }

        private IActionResult EditCentreDetailsPostRemoveLogo(EditCentreDetailsViewModel model)
        {
            ModelState.ClearAllErrors();

            ModelState.Remove(nameof(EditCentreDetailsViewModel.CentreLogo));
            model.CentreLogo = null;
            return View(model);
        }
    }
}
