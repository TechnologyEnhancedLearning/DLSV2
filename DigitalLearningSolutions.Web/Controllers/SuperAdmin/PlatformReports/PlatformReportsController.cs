namespace DigitalLearningSolutions.Web.Controllers.SuperAdmin.PlatformReports
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.SuperAdmin.PlatformReports;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredSuperAdminInterface)]
    [Authorize(Policy = CustomPolicies.UserSuperAdmin)]
    [Route("SuperAdmin/Reports")]
    [SetDlsSubApplication(nameof(DlsSubApplication.SuperAdmin))]
    [SetSelectedTab(nameof(NavMenuTab.Reports))]
    public class PlatformReportsController : Controller
    {
        private readonly IPlatformReportsService platformReportsService;
        private readonly IClockUtility clockUtility;
        private readonly IReportFilterService reportFilterService;
        public PlatformReportsController(
            IPlatformReportsService platformReportsService,
            IClockUtility clockUtility,
            IReportFilterService reportFilterService
            )
        {
            this.platformReportsService = platformReportsService;
            this.clockUtility = clockUtility;
            this.reportFilterService = reportFilterService;
        }
        public IActionResult Index()
        {
            var model = new PlatformReportsViewModel
            {
                PlatformUsageSummary = platformReportsService.GetPlatformUsageSummary()
            };
            return View(model);
        }
        [Route("NursingProficiencies")]
        public IActionResult NursingProficiencies()
        {
            var filterData = ActivityFilterData.GetDefaultFilterData(null);
            var activity = platformReportsService.GetNursingProficienciesActivity(filterData);
            var (regionName, centreName, jobGroupName, categoryName, selfAssessmentName) = reportFilterService.GetNursingAssessmentCourseFilterNames(filterData);
            var nursingReportFilterModel = new NursingReportFilterModel(
                filterData,
                jobGroupName,
                categoryName,
                regionName,
                centreName,
                selfAssessmentName,
                true
                );
            var model = new NursingProficienciesViewModel(
                activity,
                nursingReportFilterModel,
                filterData.StartDate,
                filterData.EndDate ?? clockUtility.UtcToday,
                true,
                "All"
                );
            
            return View(model);
        }

    }
}
