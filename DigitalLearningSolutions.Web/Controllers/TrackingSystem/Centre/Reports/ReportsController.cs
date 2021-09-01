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
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IUserDataService userDataService;
        private readonly ICourseDataService courseDataService;

        public ReportsController(IActivityService activityService, IJobGroupsDataService jobGroupsDataService, IUserDataService userDataService)
        {
            this.activityService = activityService;
            this.jobGroupsDataService = jobGroupsDataService;
            this.userDataService = userDataService;
        }

        public IActionResult Index()
        {
            var centreId = User.GetCentreId();
            var adminId = User.GetAdminId()!.Value;
            var adminUser = userDataService.GetAdminUserById(adminId)!;

            var filterData = new ActivityFilterData(
                DateTime.Today.AddYears(-1),
                DateTime.UtcNow,
                null,
                adminUser.CategoryId,
                null,
                ReportInterval.Months
            );
            var activity = activityService.GetFilteredActivity(centreId, filterData);

            var jobGroupName = filterData.JobGroupId.HasValue ? jobGroupsDataService.GetJobGroupName(filterData.JobGroupId.Value) : "All";
            var categoryName = adminUser.CategoryName ?? "All";
            var customisationName = filterData.CustomisationId.HasValue
                ? courseDataService.GetCourseName(filterData.CustomisationId.Value)
                : "All";
            var filterModel = new ReportsFilterModel(filterData.StartDate, filterData.EndDate, jobGroupName!, categoryName, customisationName!, filterData.ReportInterval, adminUser.CategoryId == 0);

            var model = new ReportsViewModel(activity, filterModel);
            return View(model);
        }

        [Route("Data")]
        public IEnumerable<ActivityDataRowModel> GetRecentData()
        {
            var centreId = User.GetCentreId();
            var filterData = new ActivityFilterData(
                DateTime.UtcNow.Date.AddYears(-1),
                DateTime.UtcNow,
                null,
                null,
                null,
                ReportInterval.Months
            );

            var activity = activityService.GetFilteredActivity(centreId, filterData);
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
