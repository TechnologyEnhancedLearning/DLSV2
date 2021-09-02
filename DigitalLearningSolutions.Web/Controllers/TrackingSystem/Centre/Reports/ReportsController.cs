namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using DigitalLearningSolutions.Data.Services;
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
        private readonly ICourseCategoriesDataService courseCategoriesDataService;
        private readonly ICourseDataService courseDataService;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IUserDataService userDataService;

        public ReportsController(
            IActivityService activityService,
            IJobGroupsDataService jobGroupsDataService,
            IUserDataService userDataService,
            ICourseDataService courseDataService,
            ICourseCategoriesDataService courseCategoriesDataService
        )
        {
            this.activityService = activityService;
            this.jobGroupsDataService = jobGroupsDataService;
            this.userDataService = userDataService;
            this.courseDataService = courseDataService;
            this.courseCategoriesDataService = courseCategoriesDataService;
        }

        public IActionResult Index()
        {
            var centreId = User.GetCentreId();
            var adminId = User.GetAdminId()!.Value;
            var adminUser = userDataService.GetAdminUserById(adminId)!;

            int? courseCategoryId = null;
            var validatedCategoryId = adminUser.CategoryId == 0 ? courseCategoryId : adminUser.CategoryId;

            var filterData = new ActivityFilterData(
                DateTime.UtcNow.Date.AddYears(-1),
                DateTime.UtcNow,
                null,
                validatedCategoryId,
                null,
                ReportInterval.Months
            );

            Response.Cookies.SetReportsFilterCookie(filterData, DateTime.UtcNow);

            var activity = activityService.GetFilteredActivity(centreId, filterData);

            var jobGroupName = filterData.JobGroupId.HasValue
                ? jobGroupsDataService.GetJobGroupName(filterData.JobGroupId.Value)
                : "All";
            jobGroupName ??= "All";

            var categoryName = filterData.CourseCategoryId.HasValue
                ? courseCategoriesDataService.GetCourseCategoryName(filterData.CourseCategoryId.Value)
                : "All";
            categoryName ??= "All";

            var courseNames = filterData.CustomisationId.HasValue
                ? courseDataService.GetCourseNameAndApplication(filterData.CustomisationId.Value)
                : null;
            var courseNameString = courseNames?.CompositeName ?? "All";

            var filterModel = new ReportsFilterModel(
                filterData,
                jobGroupName,
                categoryName,
                courseNameString,
                adminUser.CategoryId == 0
            );

            var model = new ReportsViewModel(activity, filterModel);
            return View(model);
        }

        [Route("Data")]
        public IEnumerable<ActivityDataRowModel> GetGraphData()
        {
            var centreId = User.GetCentreId();
            var filterData = Request.Cookies.ParseReportsFilterCookie();

            var activity = activityService.GetFilteredActivity(centreId, filterData!);
            return activity.Select(
                p => new ActivityDataRowModel(p, DateHelper.GetFormatStringForGraphLabel(p.DateInformation.Interval))
            );
        }

        // [HttpGet]
        // [Route("EditFilters")]
        // public IActionResult EditFilters()
        // {
        //     return View();
        // }
    }
}
