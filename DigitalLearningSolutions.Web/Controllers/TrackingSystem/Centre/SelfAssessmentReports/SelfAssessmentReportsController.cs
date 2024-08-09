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
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports;
    using DigitalLearningSolutions.Web.Helpers.ExternalApis;
    using System.Threading.Tasks;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Centre))]
    [Route("/TrackingSystem/Centre/Reports/SelfAssessments")]
    public class SelfAssessmentReportsController : Controller
    {
        private readonly ISelfAssessmentReportService selfAssessmentReportService;
        private readonly ITableauConnectionHelperService tableauConnectionHelper;
        private readonly IClockUtility clockUtility;

        public SelfAssessmentReportsController(
            ISelfAssessmentReportService selfAssessmentReportService,
            ITableauConnectionHelperService tableauConnectionHelper,
            IClockUtility clockUtility
        )
        {
            this.selfAssessmentReportService = selfAssessmentReportService;
            this.tableauConnectionHelper = tableauConnectionHelper;
            this.clockUtility = clockUtility;
        }
        public IActionResult Index()
        {
            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCategoryId();
            var model = new SelfAssessmentReportsViewModel(selfAssessmentReportService.GetSelfAssessmentsForReportList((int)centreId, categoryId));
            return View(model);
        }
        [HttpGet]
        [Route("DownloadDcsa")]
        public IActionResult DownloadDigitalCapabilityToExcel()
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var dataFile = selfAssessmentReportService.GetDigitalCapabilityExcelExportForCentre(centreId);
            var fileName = $"DLS DSAT Report - Centre {centreId} - downloaded {clockUtility.UtcToday:yyyy-MM-dd}.xlsx";
            return File(
                dataFile,
                FileHelper.GetContentTypeFromFileName(fileName),
                fileName
            );
        }
        [HttpGet]
        [Route("DownloadReport")]
        public IActionResult DownloadSelfAssessmentReport(int selfAssessmentId)
        {
            var centreId = User.GetCentreId();
            var dataFile = selfAssessmentReportService.GetSelfAssessmentExcelExportForCentre((int)centreId, selfAssessmentId);
            var fileName = $"Competency Self Assessment Report - Centre {centreId} - downloaded {DateTime.Today:yyyy-MM-dd}.xlsx";
            return File(
                dataFile,
                FileHelper.GetContentTypeFromFileName(fileName),
                fileName
            );
        }
        [HttpGet]
        [Route("LaunchTableauDashboards")]
        public async Task<IActionResult> LaunchTableauDashboards()
        {
            var userEmail = User.GetUserPrimaryEmail();
            var jwt = tableauConnectionHelper.GetTableauJwt(userEmail);
            var url = await tableauConnectionHelper.AuthenticateUserAsync(jwt);
            return RedirectToPage(url);
        }
    }
}
