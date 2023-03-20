using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Data.Models.Centres;
using DigitalLearningSolutions.Data.Models.Certificates;
using DigitalLearningSolutions.Data.Models.Common;
using DigitalLearningSolutions.Data.Utilities;
using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Configuration;
using DigitalLearningSolutions.Web.Helpers;
using DigitalLearningSolutions.Web.Helpers.ExternalApis;
using DigitalLearningSolutions.Web.Services;
using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Web.Controllers.Certificate
{
    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    public class CertificateController : Controller
    {
        private readonly ICentresDataService centresDataService;
        private readonly ILogger<ConfigurationController> logger;
        private readonly IMapsApiHelper mapsApiHelper;
        private readonly IImageResizeService imageResizeService;
        private ICertificateService certificateService;

        public CertificateController(
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
        [Route("Certificate/Preview/{previewId:int}")]
        public IActionResult PreviewCertificate(int previewId)
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var certificateInfo = certificateService.GetPreviewCertificateForCentre(centreId);
            if (certificateInfo == null)
            {
                return NotFound();
            }

            var model = new PreviewCertificateViewModel(certificateInfo);
            return View("Index", model);
        }
        [Route("Certificate/Activity/{progressId:int}")]
        public IActionResult ViewCertificate(int progressId)
        {
            var certificateInfo = certificateService.GetCertificateDetailsById(progressId);
            if (certificateInfo == null)
            {
                return NotFound();
            }

            var model = new PreviewCertificateViewModel(certificateInfo);
            return View("Index", model);
        }
        [Route("/Certificate/Download/{progressId:int}")]
        public async Task<IActionResult> Download(int progressId)
        {
            PdfReportStatusResponse pdfReportStatusResponse = new PdfReportStatusResponse();
            CertificateInformation certificateInfo = null;
            if (progressId == 0)
            {
                var centreId = User.GetCentreIdKnownNotNull();
                certificateInfo = certificateService.GetPreviewCertificateForCentre(centreId);
            }
            else
            {
                certificateInfo = certificateService.GetCertificateDetailsById(progressId);
            }

            if (certificateInfo == null)
            {
                return NotFound();
            }
            var model = new PreviewCertificateViewModel(certificateInfo);
            var renderedViewHTML = RenderRazorViewToString(this, "Download", model);
            var delegateId = User.GetCandidateIdKnownNotNull();
            var pdfReportResponse = await certificateService.PdfReport(certificateInfo, renderedViewHTML, delegateId);
            if (pdfReportResponse != null)
            {
                do
                {
                    pdfReportStatusResponse = await certificateService.PdfReportStatus(pdfReportResponse);
                } while (pdfReportStatusResponse.Id == 1);

                var pdfReportFile = await certificateService.GetPdfReportFile(pdfReportResponse);
                if (pdfReportFile != null)
                {
                    var fileName = $"DLS Certificate for the course - {certificateInfo.CourseName}.pdf";
                    return File(pdfReportFile, FileHelper.GetContentTypeFromFileName(fileName), fileName);
                }
            }
            return View("Index", model);
        }
        public static string RenderRazorViewToString(Controller controller, string viewName, object model = null)
        {
            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                IViewEngine viewEngine =
                    controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as
                        ICompositeViewEngine;
                ViewEngineResult viewResult = viewEngine.FindView(controller.ControllerContext, viewName, false);

                ViewContext viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    sw,
                    new HtmlHelperOptions()
                );
                viewResult.View.RenderAsync(viewContext);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}

