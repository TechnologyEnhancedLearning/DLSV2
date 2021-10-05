namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("/TrackingSystem/Centre/Reports")]
    public class ReportsController : Controller
    {
        private readonly IActivityService activityService;
        private readonly IEvaluationSummaryService evaluationSummaryService;
        private readonly IUserDataService userDataService;

        public ReportsController(
            IActivityService activityService,
            IUserDataService userDataService,
            IEvaluationSummaryService evaluationSummaryService
        )
        {
            this.activityService = activityService;
            this.userDataService = userDataService;
            this.evaluationSummaryService = evaluationSummaryService;
        }

        public IActionResult Index()
        {
            var centreId = User.GetCentreId();
            var adminId = User.GetAdminId()!.Value;
            var adminUser = userDataService.GetAdminUserById(adminId)!;

            var filterData = Request.Cookies.RetrieveFilterDataFromCookie(adminUser);

            Response.Cookies.SetReportsFilterCookie(filterData, DateTime.UtcNow);

            var activity = activityService.GetFilteredActivity(centreId, filterData);

            var (jobGroupName, courseCategoryName, courseName) = activityService.GetFilterNames(filterData);

            var filterModel = new ReportsFilterModel(
                filterData,
                jobGroupName,
                courseCategoryName,
                courseName,
                adminUser.CategoryId == 0
            );

            var evaluationResponseBreakdowns = evaluationSummaryService.GetEvaluationSummary(centreId, filterData);

            var model = new ReportsViewModel(activity, filterModel, evaluationResponseBreakdowns);
            return View(model);
        }

        [NoCaching]
        [Route("Data")]
        public ReportsChartDataModel GetGraphData()
        {
            var centreId = User.GetCentreId();
            var adminId = User.GetAdminId()!.Value;
            var adminUser = userDataService.GetAdminUserById(adminId)!;

            var filterData = Request.Cookies.RetrieveFilterDataFromCookie(adminUser);

            var activity = activityService.GetFilteredActivity(centreId, filterData!);
            var evaluationResponseBreakdowns = evaluationSummaryService.GetEvaluationSummary(centreId, filterData);

            return new ReportsChartDataModel(activity, evaluationResponseBreakdowns);
        }

        [HttpGet]
        [Route("EditFilters")]
        public IActionResult EditFilters()
        {
            var centreId = User.GetCentreId();
            var adminId = User.GetAdminId()!.Value;
            var adminUser = userDataService.GetAdminUserById(adminId)!;
            var filterData = Request.Cookies.RetrieveFilterDataFromCookie(adminUser);

            var filterOptions = GetDropdownValues(centreId, adminUser);

            var dataStartDate = activityService.GetStartOfActivityForCentre(centreId);

            var model = new EditFiltersViewModel(
                filterData,
                adminUser.CategoryId,
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
                var adminId = User.GetAdminId()!.Value;
                var adminUser = userDataService.GetAdminUserById(adminId)!;
                var filterOptions = GetDropdownValues(centreId, adminUser);
                model.SetUpDropdowns(filterOptions, adminUser.CategoryId);
                model.DataStart = activityService.GetStartOfActivityForCentre(centreId);
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
            var adminId = User.GetAdminId()!.Value;
            var adminUser = userDataService.GetAdminUserById(adminId)!;

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
                adminUser.CategoryId == 0 ? courseCategoryId : adminUser.CategoryId,
                customisationId,
                customisationId.HasValue ? CourseFilterType.Course : CourseFilterType.CourseCategory,
                reportInterval
            );

            var dataFile = activityService.GetActivityDataFileForCentre(centreId, filterData);

            return File(
                dataFile,
                FileHelper.ExcelContentType,
                $"Activity data for centre {centreId} downloaded {DateTime.Today:yyyy-MM-dd}.xlsx"
            );
        }

        private ReportsFilterOptions GetDropdownValues(
            int centreId,
            AdminUser adminUser
        )
        {
            return activityService.GetFilterOptions(
                centreId,
                adminUser.CategoryId == 0 ? (int?)null : adminUser.CategoryId
            );
        }
    }
}
