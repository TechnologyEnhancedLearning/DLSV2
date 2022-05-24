namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Centre))]
    [Route("/TrackingSystem/Centre/Reports")]
    public class ReportsController : Controller
    {
        private readonly IActivityService activityService;
        private readonly IEvaluationSummaryService evaluationSummaryService;

        public ReportsController(
            IActivityService activityService,
            IEvaluationSummaryService evaluationSummaryService
        )
        {
            this.activityService = activityService;
            this.evaluationSummaryService = evaluationSummaryService;
        }

        public IActionResult Index()
        {
            var centreId = User.GetCentreId();
            var categoryIdFilter = User.GetAdminCategoryId();

            var filterData = Request.Cookies.RetrieveFilterDataFromCookie(categoryIdFilter);

            Response.Cookies.SetReportsFilterCookie(filterData, DateTime.UtcNow);

            var activity = activityService.GetFilteredActivity(centreId, filterData);

            var (jobGroupName, courseCategoryName, courseName) = activityService.GetFilterNames(filterData);

            var filterModel = new ReportsFilterModel(
                filterData,
                jobGroupName,
                courseCategoryName,
                courseName,
                categoryIdFilter == null
            );

            var evaluationResponseBreakdowns = evaluationSummaryService.GetEvaluationSummary(centreId, filterData);

            var model = new ReportsViewModel(
                activity,
                filterModel,
                evaluationResponseBreakdowns,
                filterData.StartDate,
                filterData.EndDate ?? DateTime.Today,
                activityService.GetActivityStartDateForCentre(centreId, categoryIdFilter) != null
            );
            return View(model);
        }

        [NoCaching]
        [Route("Data")]
        public IEnumerable<ActivityDataRowModel> GetGraphData()
        {
            var centreId = User.GetCentreId();
            var categoryIdFilter = User.GetAdminCategoryId();

            var filterData = Request.Cookies.RetrieveFilterDataFromCookie(categoryIdFilter);

            var activity = activityService.GetFilteredActivity(centreId, filterData!);
            return activity.Select(
                p => new ActivityDataRowModel(p, DateHelper.GetFormatStringForGraphLabel(p.DateInformation.Interval))
            );
        }

        [HttpGet]
        [Route("EditFilters")]
        public IActionResult EditFilters()
        {
            var centreId = User.GetCentreId();
            var categoryIdFilter = User.GetAdminCategoryId();
            var filterData = Request.Cookies.RetrieveFilterDataFromCookie(categoryIdFilter);

            var filterOptions = GetDropdownValues(centreId, categoryIdFilter);

            var dataStartDate = activityService.GetActivityStartDateForCentre(centreId);

            var model = new EditFiltersViewModel(
                filterData,
                categoryIdFilter,
                filterOptions,
                dataStartDate
            );
            return View(model);
        }

        [HttpPost]
        [Route("EditFilters")]
        public IActionResult EditFilters(EditFiltersViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var centreId = User.GetCentreId();
                var categoryIdFilter = User.GetAdminCategoryId();
                var filterOptions = GetDropdownValues(centreId, categoryIdFilter);
                model.SetUpDropdowns(filterOptions, categoryIdFilter);
                model.DataStart = activityService.GetActivityStartDateForCentre(centreId);
                return View(model);
            }

            var filterData = new ActivityFilterData(
                model.GetValidatedStartDate(),
                model.GetValidatedEndDate(),
                model.JobGroupId,
                model.CourseCategoryId,
                model.CustomisationId,
                model.FilterType,
                model.ReportInterval
            );

            Response.Cookies.SetReportsFilterCookie(filterData, DateTime.UtcNow);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("Download")]
        public IActionResult DownloadUsageData(
            int? jobGroupId,
            int? courseCategoryId,
            int? customisationId,
            string startDate,
            string endDate,
            ReportInterval reportInterval
        )
        {
            var centreId = User.GetCentreId();
            var adminCategoryIdFilter = User.GetAdminCategoryId();

            var dateRange =
                activityService.GetValidatedUsageStatsDateRange(startDate, endDate, centreId);

            if (dateRange == null)
            {
                return new NotFoundResult();
            }

            var filterData = new ActivityFilterData(
                dateRange.Value.startDate,
                dateRange.Value.endDate,
                jobGroupId,
                adminCategoryIdFilter ?? courseCategoryId,
                customisationId,
                customisationId.HasValue ? CourseFilterType.Course : CourseFilterType.CourseCategory,
                reportInterval
            );

            var dataFile = activityService.GetActivityDataFileForCentre(centreId, filterData);

            var fileName = $"Activity data for centre {centreId} downloaded {DateTime.Today:yyyy-MM-dd}.xlsx";
            return File(
                dataFile,
                FileHelper.GetContentTypeFromFileName(fileName),
                fileName
            );
        }

        [HttpGet]
        [Route("DownloadEvaluationSummaries")]
        public IActionResult DownloadEvaluationSummaries(
            int? jobGroupId,
            int? courseCategoryId,
            int? customisationId,
            string startDate,
            string endDate,
            ReportInterval reportInterval
        )
        {
            var centreId = User.GetCentreId();
            var adminCategoryIdFilter = User.GetAdminCategoryId();

            var dateRange =
                activityService.GetValidatedUsageStatsDateRange(startDate, endDate, centreId);

            if (dateRange == null)
            {
                return new NotFoundResult();
            }

            var filterData = new ActivityFilterData(
                dateRange.Value.startDate,
                dateRange.Value.endDate,
                jobGroupId,
                adminCategoryIdFilter ?? courseCategoryId,
                customisationId,
                customisationId.HasValue ? CourseFilterType.Course : CourseFilterType.CourseCategory,
                reportInterval
            );

            var content = evaluationSummaryService.GetEvaluationSummaryFileForCentre(centreId, filterData);
            var fileName = $"DLS Evaluation Stats {DateTime.Today:yyyy-MM-dd}.xlsx";
            return File(
                content,
                FileHelper.GetContentTypeFromFileName(fileName),
                fileName
            );
        }

        private ReportsFilterOptions GetDropdownValues(
            int centreId,
            int? categoryIdFilter
        )
        {
            return activityService.GetFilterOptions(
                centreId,
                categoryIdFilter
            );
        }
    }
}
