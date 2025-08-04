namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.SelfAssessmentReports
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.ExternalApis;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement;
    using Microsoft.FeatureManagement.Mvc;
    using System;
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
        private readonly string tableauUrl;
        private readonly string tableauSiteName;
        private readonly string workbookName;
        private readonly string viewName;
        private readonly ISelfAssessmentService selfAssessmentService;
        private readonly IFeatureManager featureManager;
        public SelfAssessmentReportsController(
            ISelfAssessmentReportService selfAssessmentReportService,
            ITableauConnectionHelperService tableauConnectionHelper,
            IClockUtility clockUtility,
            IConfiguration config,
            ISelfAssessmentService selfAssessmentService,
            IFeatureManager featureManager
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
            this.featureManager = featureManager;
        }
        [Route("/TrackingSystem/Centre/Reports/SelfAssessments")]
        public async Task<IActionResult> IndexAsync()
        {
            var centreId = User.GetCentreId();
            var adminCategoryId = User.GetAdminCategoryId();
            var categoryId = this.selfAssessmentService.GetSelfAssessmentCategoryId(1);
            var tableauFlag = await featureManager.IsEnabledAsync(FeatureFlags.TableauSelfAssessmentDashboards);
            var tableauQueryOverride = string.Equals(Request.Query["tableaulink"], "true", StringComparison.OrdinalIgnoreCase);
            var showTableauLink = tableauFlag || tableauQueryOverride;
            var model = new SelfAssessmentReportsViewModel(selfAssessmentReportService.GetSelfAssessmentsForReportList((int)centreId, adminCategoryId), adminCategoryId, categoryId, showTableauLink);
            return View(model);
        }
        [HttpGet]
        [Route("/TrackingSystem/Centre/Reports/DownloadDcsa")]
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
        [Route("/TrackingSystem/Centre/Reports/DownloadReport")]
        public IActionResult DownloadSelfAssessmentReport(int selfAssessmentId)
        {
            var centreId = User.GetCentreId();
            var selfAssessmentName = selfAssessmentService.GetSelfAssessmentNameById(selfAssessmentId);
            var dataFile = selfAssessmentReportService.GetSelfAssessmentExcelExportForCentre((int)centreId, selfAssessmentId);
            var fileName = $"{((selfAssessmentName.Length > 50) ? selfAssessmentName.Substring(0, 50) : selfAssessmentName)} Report - Centre {centreId} - downloaded {clockUtility.UtcNow:yyyy-MM-dd}.xlsx";
            return File(
                dataFile,
                FileHelper.GetContentTypeFromFileName(fileName),
                fileName
            );
        }
        [HttpGet]
        [Route("/{source}/Reports/TableauCompetencyDashboard")]
        public async Task<IActionResult> TableauCompetencyDashboardAsync(string source = "TrackingSystem")
        {
            var userEmail = User.GetUserPrimaryEmail();
            var adminId = User.GetAdminId();
            var jwt = tableauConnectionHelper.GetTableauJwt();
            var tableauFlag = await featureManager.IsEnabledAsync(FeatureFlags.TableauSelfAssessmentDashboards);
            var tableauQueryOverride = string.Equals(Request.Query["tableaulink"], "true", StringComparison.OrdinalIgnoreCase);
            var showTableauLink = tableauFlag || tableauQueryOverride;
            ViewBag.Source = source;
            ViewBag.Email = userEmail;
            ViewBag.AdminId = adminId;
            ViewBag.SiteName = tableauSiteName;
            ViewBag.TableauServerUrl = tableauUrl;
            ViewBag.WorkbookName = workbookName;
            ViewBag.ViewName = viewName;
            ViewBag.JwtToken = jwt;
            ViewBag.ShowTableauLink = showTableauLink;
            return View();
        }
    }
}
