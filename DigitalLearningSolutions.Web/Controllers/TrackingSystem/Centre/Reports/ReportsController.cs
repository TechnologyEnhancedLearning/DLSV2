namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports;
    using DocumentFormat.OpenXml.Vml.Spreadsheet;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Centre))]
    [Route("/TrackingSystem/Centre/Reports/Courses")]
    public class ReportsController : Controller
    {
        private readonly IActivityService activityService;
        private readonly IEvaluationSummaryService evaluationSummaryService;
        private readonly IClockUtility clockUtility;
        private readonly IReportFilterService reportFilterService;

        public ReportsController(
            IActivityService activityService,
            IEvaluationSummaryService evaluationSummaryService,
            IClockUtility clockUtility,
            IReportFilterService reportFilterService
        )
        {
            this.activityService = activityService;
            this.evaluationSummaryService = evaluationSummaryService;
            this.clockUtility = clockUtility;
            this.reportFilterService = reportFilterService;
        }

        public IActionResult Index()
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var categoryIdFilter = User.GetAdminCategoryId();

            var filterData = Request.Cookies.RetrieveFilterDataFromCookie("ReportsFilterCookie", categoryIdFilter);

            Response.Cookies.SetReportsFilterCookie("ReportsFilterCookie", filterData, clockUtility.UtcNow);

            var activity = activityService.GetFilteredActivity(centreId, filterData);

            var (jobGroupName, courseCategoryName, courseName) = reportFilterService.GetFilterNames(filterData);

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
                filterData.EndDate ?? clockUtility.UtcToday,
                activityService.GetActivityStartDateForCentre(centreId, categoryIdFilter) != null,
                reportFilterService.GetCourseCategoryNameForActivityFilter(categoryIdFilter)
            );
            return View(model);
        }

        [NoCaching]
        [Route("Data")]
        public IEnumerable<ActivityDataRowModel> GetGraphData()
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var categoryIdFilter = User.GetAdminCategoryId();

            var filterData = Request.Cookies.RetrieveFilterDataFromCookie("ReportsFilterCookie", categoryIdFilter);

            var activity = activityService.GetFilteredActivity(centreId, filterData!);
            return activity.Select(
                p => new ActivityDataRowModel(p, DateHelper.GetFormatStringForGraphLabel(p.DateInformation.Interval))
            );
        }

        [HttpGet]
        [Route("EditFilters")]
        public IActionResult EditFilters()
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var categoryIdFilter = User.GetAdminCategoryId();
            var filterData = Request.Cookies.RetrieveFilterDataFromCookie("ReportsFilterCookie", categoryIdFilter);

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
            var categoryIdFilter = User.GetAdminCategoryId();
            if (!ModelState.IsValid)
            {
                var centreId = User.GetCentreIdKnownNotNull();
                var filterOptions = GetDropdownValues(centreId, categoryIdFilter);
                model.SetUpDropdowns(filterOptions, categoryIdFilter);
                model.DataStart = activityService.GetActivityStartDateForCentre(centreId);
                return View(model);
            }

            var filterData = new ActivityFilterData(
                model.GetValidatedStartDate(),
                model.GetValidatedEndDate(),
                model.JobGroupId,
                categoryIdFilter ?? model.CourseCategoryId,
                model.CustomisationId,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                model.FilterType,
                model.ReportInterval
            );

            Response.Cookies.SetReportsFilterCookie("ReportsFilterCookie", filterData, clockUtility.UtcNow);

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
            var centreId = User.GetCentreIdKnownNotNull();
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
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                customisationId.HasValue ? CourseFilterType.Activity : CourseFilterType.Category,
                reportInterval
            );

            var dataFile = activityService.GetActivityDataFileForCentre(centreId, filterData);

            var fileName = $"Activity data for centre {centreId} downloaded {clockUtility.UtcToday:yyyy-MM-dd}.xlsx";
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
            var centreId = User.GetCentreIdKnownNotNull();
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
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                customisationId.HasValue ? CourseFilterType.Activity : CourseFilterType.Category,
                reportInterval
            );

            var content = evaluationSummaryService.GetEvaluationSummaryFileForCentre(centreId, filterData);
            var fileName = $"DLS Evaluation Stats {clockUtility.UtcToday:yyyy-MM-dd}.xlsx";
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
            return reportFilterService.GetFilterOptions(
                centreId,
                categoryIdFilter
            );
        }
    }
}
