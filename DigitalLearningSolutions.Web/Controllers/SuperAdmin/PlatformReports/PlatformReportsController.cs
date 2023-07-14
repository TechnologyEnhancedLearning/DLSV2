namespace DigitalLearningSolutions.Web.Controllers.SuperAdmin.PlatformReports
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.PlatformReports;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.SuperAdmin.PlatformReports;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports;
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
            var filterData = Request.Cookies.RetrieveFilterDataFromCookie("ReportsFilterCookie", null);
            Response.Cookies.SetReportsFilterCookie("ReportsFilterCookie", filterData, clockUtility.UtcNow);
            var activity = platformReportsService.GetNursingProficienciesActivity(filterData);
            var (regionName, centreTypeName, centreName, jobGroupName, brandName, categoryName, selfAssessmentName) = reportFilterService.GetNursingAssessmentCourseFilterNames(filterData);
            var nursingReportFilterModel = new NursingReportFilterModel(
                filterData,
                regionName,
                centreTypeName,
                centreName,
                jobGroupName,
                brandName,
                categoryName,
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
        [HttpGet]
        [Route("NursingProficiencies/EditFilters")]
        public IActionResult NursingReportEditFilters()
        {
            var filterData = Request.Cookies.RetrieveFilterDataFromCookie("SuperAdminReportsFilterCookie", null);
            var filterOptions = GetDropdownValues();
            var dataStartDate = platformReportsService.GetNursingProficienciesActivityStartDate();
            var model = new NursingReportEditFiltersViewModel(
                filterData,
                null,
                filterOptions,
                dataStartDate
            );
            return View("NursingReportEditFilters", model);
        }

        [HttpPost]
        [Route("NursingProficiencies/EditFilters")]
        public IActionResult NursingReportEditFilters(NursingReportEditFiltersViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var filterOptions = GetDropdownValues();
                model.DataStart = platformReportsService.GetNursingProficienciesActivityStartDate();
                return View("NursingReportEditFilters", model);
            }

            var filterData = new ActivityFilterData(
                model.GetValidatedStartDate(),
                model.GetValidatedEndDate(),
                model.JobGroupId,
                model.CategoryId,
                null,
                model.RegionId,
                model.CentreId,
                model.SelfAssessmentId,
                model.CentreTypeId,
                model.BrandId,
                model.FilterType,
                model.ReportInterval
            );

            Response.Cookies.SetReportsFilterCookie("SuperAdminReportsFilterCookie", filterData, clockUtility.UtcNow);

            return RedirectToAction("NursingProficiencies");
        }
        private NursingReportsFilterOptions GetDropdownValues()
        {
            return reportFilterService.GetNursingFilterOptions();
        }
    }
}
