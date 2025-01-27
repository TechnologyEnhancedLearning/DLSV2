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
    using Microsoft.Extensions.Configuration;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Web.Services;

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
        private readonly string tableauUrl;
        private readonly string tableauSiteName;
        private readonly string workbookName;
        private readonly string viewName;
        private readonly ISelfAssessmentService selfAssessmentService;
        public SelfAssessmentReportsController(
            ISelfAssessmentReportService selfAssessmentReportService,
            ITableauConnectionHelperService tableauConnectionHelper,
            IClockUtility clockUtility,
            IConfiguration config,
            ISelfAssessmentService selfAssessmentService
        )
        {
            this.selfAssessmentReportService = selfAssessmentReportService;
            this.tableauConnectionHelper = tableauConnectionHelper;
            this.clockUtility = clockUtility;
            tableauUrl = config.GetTableauSiteUrl();
            tableauSiteName = config.GetTableauSiteName();
            workbookName = config.GetTableauWorkbookName();
            viewName = config.GetTableauViewName();
            this.selfAssessmentService = selfAssessmentService;
        }
        public IActionResult Index()
        {
            var centreId = User.GetCentreId();
            var adminCategoryId = User.GetAdminCategoryId();
            var categoryId = this.selfAssessmentService.GetSelfAssessmentCategoryId(1);
            var model = new SelfAssessmentReportsViewModel(selfAssessmentReportService.GetSelfAssessmentsForReportList((int)centreId, adminCategoryId), adminCategoryId, categoryId);
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
        [Route("TableauCompetencyDashboard")]
        public IActionResult TableauCompetencyDashboard()
        {
            var userEmail = User.GetUserPrimaryEmail();
            var jwt = tableauConnectionHelper.GetTableauJwt(userEmail);
            ViewBag.SiteName = tableauSiteName;
            ViewBag.TableauServerUrl = tableauUrl;
            ViewBag.WorkbookName = workbookName;
            ViewBag.ViewName = viewName;
            ViewBag.JwtToken = jwt;

            return View();
        }
    }
}
