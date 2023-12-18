namespace DigitalLearningSolutions.Web.Controllers.SuperAdmin.PlatformReports
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.PlatformReports;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.SuperAdmin.PlatformReports;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;
    using System.Collections.Generic;
    using System.Linq;

    [FeatureGate(FeatureFlags.RefactoredSuperAdminInterface)]
    [Authorize(Policy = CustomPolicies.UserSuperAdmin)]
    [Route("SuperAdmin/Reports")]
    [SetDlsSubApplication(nameof(DlsSubApplication.SuperAdmin))]
    [SetSelectedTab(nameof(NavMenuTab.Reports))]
    public partial class PlatformReportsController : Controller
    {
        private readonly IPlatformReportsService platformReportsService;
        private readonly IClockUtility clockUtility;
        private readonly IReportFilterService reportFilterService;
        private readonly IPlatformUsageSummaryDownloadFileService platformUsageSummaryDownloadFileService = null!;

        private SelfAssessmentReportsFilterOptions GetDropdownValues(bool supervised)
        {
            return reportFilterService.GetSelfAssessmentFilterOptions(supervised);
        }

        public PlatformReportsController(
            IPlatformReportsService platformReportsService,
            IClockUtility clockUtility,
            IReportFilterService reportFilterService,
            IPlatformUsageSummaryDownloadFileService platformUsageSummaryDownloadFileService
            )
        {
            this.platformReportsService = platformReportsService;
            this.clockUtility = clockUtility;
            this.reportFilterService = reportFilterService;
            this.platformUsageSummaryDownloadFileService = platformUsageSummaryDownloadFileService;
        }

        public IActionResult Index()
        {
            var model = new PlatformReportsViewModel
            {
                PlatformUsageSummary = platformReportsService.GetPlatformUsageSummary()
            };
            return View(model);
        }

        [NoCaching]
        [Route("SelfAssessments/{selfAssessmentType}/Data")]
        public IEnumerable<SelfAssessmentActivityDataRowModel> GetGraphData(string selfAssessmentType)
        {
            var cookieName = selfAssessmentType == "Independent" ? "SuperAdminIndependentSAReportsFilterCookie" : "SuperAdminSupervisedSAReportsFilterCookie";
            var filterData = Request.Cookies.RetrieveFilterDataFromCookie(cookieName, null);
            var activity = platformReportsService.GetSelfAssessmentActivity(filterData!, selfAssessmentType == "Independent" ? false : true);
            return activity.Select(
                p => new SelfAssessmentActivityDataRowModel(p, DateHelper.GetFormatStringForGraphLabel(p.DateInformation.Interval))
            );
        }
        [Route("Export")]
        public IActionResult Export()
        {
            var content = this.platformUsageSummaryDownloadFileService.GetPlatformUsageSummaryFile();

            var  fileName = $"Report platform usage summary {this.clockUtility.UtcNow:yyyy-MM-dd}.xlsx";
            return File(
                content,
                FileHelper.GetContentTypeFromFileName(fileName),
                fileName
            );
        }
    }
}
