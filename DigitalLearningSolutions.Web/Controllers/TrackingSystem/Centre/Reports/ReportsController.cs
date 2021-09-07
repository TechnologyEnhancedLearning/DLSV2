namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
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
        private readonly IUserDataService userDataService;

        public ReportsController(
            IActivityService activityService,
            IUserDataService userDataService
        )
        {
            this.activityService = activityService;
            this.userDataService = userDataService;
        }

        public IActionResult Index()
        {
            var centreId = User.GetCentreId();
            var adminId = User.GetAdminId()!.Value;
            var adminUser = userDataService.GetAdminUserById(adminId)!;

            var filterData = ActivityFilterData.GetDefaultFilterData(adminUser);

            Response.Cookies.SetReportsFilterCookie(filterData, DateTime.UtcNow);

            var activity = activityService.GetFilteredActivity(centreId, filterData);

            var jobGroupNameString = activityService.GetJobGroupNameForActivityFilter(filterData.JobGroupId);
            var categoryNameString = activityService.GetCourseCategoryNameForActivityFilter(filterData.CourseCategoryId);
            var courseNameString = activityService.GetCourseNameForActivityFilter(filterData.CustomisationId);

            var filterModel = new ReportsFilterModel(
                filterData,
                jobGroupNameString,
                categoryNameString,
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
            var adminId = User.GetAdminId()!.Value;
            var adminUser = userDataService.GetAdminUserById(adminId)!;

            var filterData = Request.Cookies.ParseReportsFilterCookie(adminUser);

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
