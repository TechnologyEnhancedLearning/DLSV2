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
    public class PlatformReportsController : Controller
    {
        private readonly IPlatformReportsService platformReportsService;
        private readonly IClockUtility clockUtility;
        private readonly IReportFilterService reportFilterService;
        private SelfAssessmentReportsFilterOptions GetDropdownValues(bool supervised)
        {
            return reportFilterService.GetSelfAssessmentFilterOptions(supervised);
        }
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
        [Route("SelfAssessments/Supervised")]
        public IActionResult SupervisedSelfAssessmentsReport()
        {
            var filterData = Request.Cookies.RetrieveFilterDataFromCookie("SuperAdminReportsFilterCookie", null);
            Response.Cookies.SetReportsFilterCookie("SuperAdminReportsFilterCookie", filterData, clockUtility.UtcNow);
            var activity = platformReportsService.GetSelfAssessmentActivity(filterData, true);
            var (regionName, centreTypeName, centreName, jobGroupName, brandName, categoryName, selfAssessmentName) = reportFilterService.GetSelfAssessmentFilterNames(filterData);
            var selfAssessmentReportFilterModel = new SelfAssessmentReportFilterModel(
                filterData,
                regionName,
                centreTypeName,
                centreName,
                jobGroupName,
                brandName,
                categoryName,
                selfAssessmentName,
                true,
                true
                );
            var model = new SelfAssessmentsReportViewModel(
                activity,
                selfAssessmentReportFilterModel,
                filterData.StartDate,
                filterData.EndDate ?? clockUtility.UtcToday,
                true,
                "All",
                true
                );

            return View("SelfAssessmentsReport", model);
        }
        [NoCaching]
        [Route("SelfAssessments/{selfAssessmentType}/Data")]
        public IEnumerable<SelfAssessmentActivityDataRowModel> GetGraphData(string selfAssessmentType)
        {
            var cookieName = selfAssessmentType == "Independent" ? "SuperAdminDSATReportsFilterCookie" : "SuperAdminReportsFilterCookie";
            var filterData = Request.Cookies.RetrieveFilterDataFromCookie(cookieName, null);
            var activity = platformReportsService.GetSelfAssessmentActivity(filterData!, selfAssessmentType == "Independent" ? false : true);
            return activity.Select(
                p => new SelfAssessmentActivityDataRowModel(p, DateHelper.GetFormatStringForGraphLabel(p.DateInformation.Interval))
            );
        }
        [HttpGet]
        [Route("SelfAssessments/Supervised/EditFilters")]
        public IActionResult SupervisedSelfAssessmentsEditFilters()
        {
            var filterData = Request.Cookies.RetrieveFilterDataFromCookie("SuperAdminReportsFilterCookie", null);
            var filterOptions = GetDropdownValues(true);
            var dataStartDate = platformReportsService.GetSelfAssessmentActivityStartDate(true);
            var model = new SelfAssessmentsEditFiltersViewModel(
                filterData,
                null,
                filterOptions,
                dataStartDate,
                true
            );
            return View("SelfAssessmentsEditFilters", model);
        }

        [HttpPost]
        [Route("SelfAssessments/Supervised/EditFilters")]
        public IActionResult SupervisedSelfAssessmentsEditFilters(SelfAssessmentsEditFiltersViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var filterOptions = GetDropdownValues(true);
                model.DataStart = platformReportsService.GetSelfAssessmentActivityStartDate(true);
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

            return RedirectToAction("SupervisedSelfAssessmentsReport");
        }

        [Route("SelfAssessments/Independent")]
        public IActionResult IndependentSelfAssessmentsReport()
        {
            var filterData = Request.Cookies.RetrieveFilterDataFromCookie("SuperAdminDSATReportsFilterCookie", null);
            Response.Cookies.SetReportsFilterCookie("SuperAdminDSATReportsFilterCookie", filterData, clockUtility.UtcNow);
            var activity = platformReportsService.GetSelfAssessmentActivity(filterData, false);
            var (regionName, centreTypeName, centreName, jobGroupName, brandName, categoryName, selfAssessmentName) = reportFilterService.GetSelfAssessmentFilterNames(filterData);
            var selfAssessmentReportFilterModel = new SelfAssessmentReportFilterModel(
                filterData,
                regionName,
                centreTypeName,
                centreName,
                jobGroupName,
                brandName,
                categoryName,
                selfAssessmentName,
                true,
                false
                );
            var model = new SelfAssessmentsReportViewModel(
                activity,
                selfAssessmentReportFilterModel,
                filterData.StartDate,
                filterData.EndDate ?? clockUtility.UtcToday,
                true,
                "All",
                false
                );

            return View("SelfAssessmentsReport", model);
        }

        [HttpGet]
        [Route("SelfAssessments/Independent/EditFilters")]
        public IActionResult IndependentSelfAssessmentsEditFilters()
        {
            var filterData = Request.Cookies.RetrieveFilterDataFromCookie("SuperAdminDSATReportsFilterCookie", null);
            var filterOptions = GetDropdownValues(false);
            var dataStartDate = platformReportsService.GetSelfAssessmentActivityStartDate(false);
            var model = new SelfAssessmentsEditFiltersViewModel(
                filterData,
                null,
                filterOptions,
                dataStartDate,
                false
            );
            return View("SelfAssessmentsEditFilters", model);
        }

        [HttpPost]
        [Route("SelfAssessments/Independent/EditFilters")]
        public IActionResult IndependentSelfAssessmentsEditFilters(SelfAssessmentsEditFiltersViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var filterOptions = GetDropdownValues(false);
                model.DataStart = platformReportsService.GetSelfAssessmentActivityStartDate(false);
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
            Response.Cookies.SetReportsFilterCookie("SuperAdminDSATReportsFilterCookie", filterData, clockUtility.UtcNow);

            return RedirectToAction("IndependentSelfAssessmentsReport");
        }
    }
}
