namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.SelfAssessmentReports
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Data.Enums;
    using System;
    using DigitalLearningSolutions.Data.Utilities;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Centre))]
    [Route("/TrackingSystem/Centre/Reports/SelfAssessments")]
    public class SelfAssessmentReportsController : Controller
    {
        private readonly ISelfAssessmentReportService selfAssessmentReportService;
        private readonly IClockUtility clockUtility;

        public SelfAssessmentReportsController(
            ISelfAssessmentReportService selfAssessmentReportService,
            IClockUtility clockUtility
        )
        {
            this.selfAssessmentReportService = selfAssessmentReportService;
            this.clockUtility = clockUtility;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Route("Download")]
        public IActionResult DownloadDigitalCapabilityToExcel()
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var dataFile = selfAssessmentReportService.GetDigitalCapabilityExcelExportForCentre(centreId);
            var fileName = $"DLS DCSA Report - Centre {centreId} - downloaded {clockUtility.UtcToday:yyyy-MM-dd}.xlsx";
            return File(
                dataFile,
                FileHelper.GetContentTypeFromFileName(fileName),
                fileName
            );
        }
    }
}
