namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Configuration;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    public class PdfDownloadController : Controller
    {
        private readonly ICertificateService certificateService;

        public PdfDownloadController(ICertificateService certificateService)
        {
            this.certificateService = certificateService;
        }

        public IActionResult PreviewCertificate(int centreId)
        {
            var certificateInfo = certificateService.GetPreviewCertificateForCentre(centreId);
            if (certificateInfo == null)
            {
                return NotFound();
            }

            var model = new PreviewCertificateViewModel(certificateInfo, centreId);
            return View(model);
        }
    }
}
