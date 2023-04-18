namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Configuration
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.ExternalApis;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Configuration;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Centre))]
    [Route("/TrackingSystem/Centre/Configuration")]
    public class ConfigurationController : Controller
    {
        private readonly ICentresDataService centresDataService;
        private readonly ILogger<ConfigurationController> logger;
        private readonly IMapsApiHelper mapsApiHelper;
        private readonly IImageResizeService imageResizeService;
        private ICertificateService certificateService;

        public ConfigurationController(
            ICentresDataService centresDataService,
            IMapsApiHelper mapsApiHelper,
            ILogger<ConfigurationController> logger,
            IImageResizeService imageResizeService,
            ICertificateService certificateService
        )
        {
            this.centresDataService = centresDataService;
            this.mapsApiHelper = mapsApiHelper;
            this.logger = logger;
            this.imageResizeService = imageResizeService;
            this.certificateService = certificateService;
        }

        public IActionResult Index()
        {
            var centreId = User.GetCentreIdKnownNotNull();

            var centreDetails = centresDataService.GetCentreDetailsById(centreId)!;

            var model = new CentreConfigurationViewModel(centreDetails);

            return View("Index", model);
        }

        [HttpGet]
        [Route("EditCentreManagerDetails")]
        public IActionResult EditCentreManagerDetails()
        {
            var centreId = User.GetCentreIdKnownNotNull();

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

            var centreId = User.GetCentreIdKnownNotNull();

            centresDataService
                .UpdateCentreManagerDetails(centreId, model.FirstName!, model.LastName!, model.Email!, model.Telephone);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("EditCentreWebsiteDetails")]
        public IActionResult EditCentreWebsiteDetails()
        {
            var centreId = User.GetCentreIdKnownNotNull();

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
                return new StatusCodeResult(500);
            }

            var latitude = double.Parse(mapsResponse.Results[0].Geometry.Location.Latitude);
            var longitude = double.Parse(mapsResponse.Results[0].Geometry.Location.Longitude);

            var centreId = User.GetCentreIdKnownNotNull();

            centresDataService.UpdateCentreWebsiteDetails(
                centreId,
                model.CentrePostcode,
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
            var centreId = User.GetCentreIdKnownNotNull();

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

        [Route("PreviewCertificate")]
        public IActionResult PreviewCertificate()
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var certificateInfo = certificateService.GetPreviewCertificateForCentre(centreId);
            if (certificateInfo == null)
            {
                return NotFound();
            }

            var model = new PreviewCertificateViewModel(certificateInfo);
            return View(model);
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

            var centreId = User.GetCentreIdKnownNotNull();

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
